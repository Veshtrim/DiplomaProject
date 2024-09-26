using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using TemaEDiplomesUBT.Workflow.WorkflowData;

namespace TemaEDiplomesUBT.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkflowHost _workflow;

        public PurchaseService(ApplicationDbContext context, IWorkflowHost workflow)
        {
            _context = context;
            _workflow = workflow;
        }

        public async Task<List<Purchase>> GetAllPurchasesAsync()
        {
            return await _context.Purchases
                .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Product)
                .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Warehouse)
                .Include(p => p.Supplier)  
                .ToListAsync();
        }

        public async Task<PurchaseViewModel> GetPurchaseByIdAsync(int purchaseId)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseDetails)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);

            if (purchase == null)
            {
                return null;
            }

            var productIds = purchase.PurchaseDetails.Select(pd => pd.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();

            var warehouseIds = purchase.PurchaseDetails.Select(pd => pd.WarehouseId).Distinct().ToList();
            var warehouses = await _context.Warehouses
                .Where(w => warehouseIds.Contains(w.Id))
                .Select(w => new { w.Id, w.Name })
                .ToListAsync();

            return new PurchaseViewModel
            {
                PurchaseId = purchase.PurchaseId,
                PurchaseDate = purchase.PurchaseDate,
                TotalAmount = purchase.TotalAmount,
                PurchaseDocumentNumber = purchase.PurchaseDocumentNumber,
                IsPaid = purchase.IsPaid,
                SupplierId = purchase.SupplierId,
                SupplierName = purchase.Supplier?.Name,
                PurchaseDetails = purchase.PurchaseDetails.Select(pd => new PurchaseDetailViewModel
                {
                    ProductId = pd.ProductId,
                    ProductName = products.FirstOrDefault(p => p.Id == pd.ProductId)?.Name ?? "Unknown",
                    WarehouseId = pd.WarehouseId,
                    WarehouseName = warehouses.FirstOrDefault(w => w.Id == pd.WarehouseId)?.Name ?? "Unknown",
                    Quantity = pd.Quantity,
                    PurchasePrice = pd.PurchasePrice
                }).ToList()
            };
        }


        public async Task<WorkflowResult> CreatePurchaseAsync(PurchaseViewModel model)
        {
            try
            {
                foreach (var detail in model.PurchaseDetails)
                {
                    var stock = await _context.Stocks
                        .Include(s => s.Product)
                        .Include(s => s.Warehouse)
                        .FirstOrDefaultAsync(s => s.ProductId == detail.ProductId && s.WarehouseId == detail.WarehouseId);

                    if (stock == null)
                    {
                        return new WorkflowResult
                        {
                            IsSuccessful = false,
                            ErrorMessage = $"Stock item not found for Product: {detail.ProductId} and Warehouse: {detail.WarehouseId}"
                        };
                    }
                    detail.ProductName = stock.Product?.Name ?? "Unknown";
                    detail.WarehouseName = stock.Warehouse?.Name ?? "Unknown";
                    if (stock.Quantity <= 0 || stock.Quantity < detail.Quantity)
                    {
                        return new WorkflowResult
                        {
                            IsSuccessful = false,
                            ErrorMessage = $"Insufficient stock for Product: {detail.ProductName} in Warehouse: {detail.WarehouseName}"
                        };
                    }
                }

                var warehouseId = model.PurchaseDetails.First().WarehouseId;
                var purchasesInWarehouse = await _context.Purchases
                    .Where(p => p.PurchaseDetails.Any(pd => pd.WarehouseId == warehouseId))
                    .CountAsync();

                int nextPurchaseNumber = purchasesInWarehouse + 1;
                var warehouseCode = warehouseId.ToString("D3");
                var documentNumber = $"{nextPurchaseNumber}-{warehouseCode}-P1AP";
                var supplier = await _context.Suppliers.FindAsync(model.SupplierId);
                string supplierName = supplier?.Name ?? "Unknown Customer";
                var purchase = new Purchase
                {
                    PurchaseDate = model.PurchaseDate,
                    TotalAmount = model.TotalAmount,
                    PurchaseDocumentNumber = documentNumber,
                    IsPaid = model.IsPaid,
                    SupplierId = model.SupplierId,
                    PurchaseDetails = model.PurchaseDetails.Select(pd => new PurchaseDetail
                    {
                        ProductId = pd.ProductId,
                        WarehouseId = pd.WarehouseId,
                        Quantity = pd.Quantity,
                        PurchasePrice = pd.PurchasePrice
                    }).ToList()
                };

                var workflowData = new PurchaseWorkflowData
                {
                    PurchaseDate = model.PurchaseDate,
                    TotalAmount = model.TotalAmount,
                    PurchaseDetails = model.PurchaseDetails,
                    PurchaseDocumentNumber = documentNumber,
                    SupplierName = supplierName
                };

                var workflowInstance = await _workflow.StartWorkflow("PurchaseWorkflow", workflowData);

                var result = await WaitForWorkflowToComplete(workflowInstance);
                if (!result.IsSuccessful)
                {
                    return new WorkflowResult
                    {
                        IsSuccessful = false,
                        ErrorMessage = result.ErrorMessage
                    };
                }

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                return new WorkflowResult
                {
                    IsSuccessful = true
                };
            }
            catch (Exception ex)
            {
                return new WorkflowResult
                {
                    IsSuccessful = false,
                    ErrorMessage = "Error processing purchase: " + ex.Message
                };
            }
        }

        public async Task UpdatePurchaseAsync(PurchaseViewModel purchaseViewModel)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseViewModel.PurchaseId);

            if (purchase == null)
            {
                throw new ArgumentException("Purchase not found.");
            }

            var oldPurchaseDetails = purchase.PurchaseDetails.ToList();
            var productIds = oldPurchaseDetails.Select(sd => sd.ProductId).Union(purchaseViewModel.PurchaseDetails.Select(sd => sd.ProductId)).Distinct().ToList();
            var warehouseIds = oldPurchaseDetails.Select(sd => sd.WarehouseId).Union(purchaseViewModel.PurchaseDetails.Select(sd => sd.WarehouseId)).Distinct().ToList();

            var products = await _context.Products
               .Where(p => productIds.Contains(p.Id))
               .ToDictionaryAsync(p => p.Id, p => p.Name);

            var warehouses = await _context.Warehouses
                .Where(w => warehouseIds.Contains(w.Id))
                .ToDictionaryAsync(w => w.Id, w => w.Name);

            var documentNumber = purchase.PurchaseDocumentNumber;
            var supplier = await _context.Suppliers.FindAsync(purchaseViewModel.SupplierId);
            string supplierName = supplier?.Name ?? "Unknown Customer";
            var workflowData = new PurchaseWorkflowData
            {
                OldPurchase = oldPurchaseDetails.Select(sd => new PurchaseDetailViewModel
                {
                    ProductId = sd.ProductId,
                    ProductName = products.ContainsKey(sd.ProductId) ? products[sd.ProductId] : "Unknown",
                    WarehouseId = sd.WarehouseId,
                    WarehouseName = warehouses.ContainsKey(sd.WarehouseId) ? warehouses[sd.WarehouseId] : "Unknown",
                    Quantity = sd.Quantity,
                    PurchasePrice = sd.PurchasePrice
                }).ToList(),
                NewPurchase = purchaseViewModel.PurchaseDetails.Select(sd => new PurchaseDetailViewModel
                {
                    ProductId = sd.ProductId,
                    ProductName = products.ContainsKey(sd.ProductId) ? products[sd.ProductId] : "Unknown",
                    WarehouseId = sd.WarehouseId,
                    WarehouseName = warehouses.ContainsKey(sd.WarehouseId) ? warehouses[sd.WarehouseId] : "Unknown",
                    Quantity = sd.Quantity,
                    PurchasePrice = sd.PurchasePrice
                }).ToList(),
                PurchaseDocumentNumber = documentNumber,
                SupplierName = supplierName
            };

            var workflowInstance = await _workflow.StartWorkflow("UpdatePurchaseWorkflow", workflowData);
            var result = await WaitForWorkflowToComplete(workflowInstance);

            if (!result.IsSuccessful)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }

            purchase.PurchaseDate = purchaseViewModel.PurchaseDate;
            purchase.TotalAmount = purchaseViewModel.TotalAmount;
            purchase.PurchaseDocumentNumber = purchaseViewModel.PurchaseDocumentNumber;
            purchase.IsPaid = purchaseViewModel.IsPaid;
            purchase.SupplierId = purchaseViewModel.SupplierId; 

            foreach (var oldDetail in oldPurchaseDetails)
            {
                if (!purchaseViewModel.PurchaseDetails.Any(sd => sd.ProductId == oldDetail.ProductId && sd.WarehouseId == oldDetail.WarehouseId))
                {
                    _context.PurchaseDetails.Remove(oldDetail);
                }
            }

            foreach (var detail in purchaseViewModel.PurchaseDetails)
            {
                var existingDetail = oldPurchaseDetails.FirstOrDefault(sd => sd.ProductId == detail.ProductId && sd.WarehouseId == detail.WarehouseId);

                if (existingDetail != null)
                {
                    existingDetail.Quantity = detail.Quantity;
                    existingDetail.PurchasePrice = detail.PurchasePrice;
                }
                else
                {
                    var purchaseDetail = new PurchaseDetail
                    {
                        ProductId = detail.ProductId,
                        WarehouseId = detail.WarehouseId,
                        Quantity = detail.Quantity,
                        PurchasePrice = detail.PurchasePrice
                    };
                    purchase.PurchaseDetails.Add(purchaseDetail);
                }

                purchase.TotalAmount += detail.Quantity * detail.PurchasePrice;
            }

            _context.Update(purchase);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePurchaseAsync(int id)
        {
            var purchaseViewModel = await GetPurchaseByIdAsync(id);
            if (purchaseViewModel == null) return;

            var purchase = await _context.Purchases
                .Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();
        }

        public async Task<WorkflowResult> WaitForWorkflowToComplete(string workflowId)
        {
            while (true)
            {
                var workflowInstance = await _workflow.PersistenceStore.GetWorkflowInstance(workflowId);

                if (workflowInstance.Status == WorkflowStatus.Complete)
                {
                    return new WorkflowResult { IsSuccessful = true };
                }

                if (workflowInstance.Status == WorkflowStatus.Terminated || workflowInstance.Status == WorkflowStatus.Suspended)
                {
                    var errorMessage = workflowInstance.Data as string;
                    return new WorkflowResult { IsSuccessful = false, ErrorMessage = errorMessage };
                }

                await Task.Delay(1000);
            }
        }
    }
}
