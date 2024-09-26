using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Services.IServices;
using Microsoft.EntityFrameworkCore;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TemaEDiplomesUBT.Workflow.WorkflowData;

namespace TemaEDiplomesUBT.Services
{
    public class SaleService : ISaleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkflowHost _workflow;

        public SaleService(ApplicationDbContext context, IWorkflowHost workflow)
        {
            _context = context;
            _workflow = workflow;
        }

        public async Task<List<Sale>> GetAllSalesAsync()
        {
            return await _context.Sales
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Warehouse)
                .Include(s => s.Customer)
                .Include(s => s.Payments) 
                .ToListAsync();
        }

        public async Task<SaleViewModel> GetSaleByIdAsync(int saleId)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .Include(s => s.Customer)
                .Include(s => s.Payments) 
                .FirstOrDefaultAsync(s => s.SaleId == saleId);

            if (sale == null)
            {
                return null;
            }

            var productIds = sale.SaleDetails.Select(sd => sd.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();

            var warehouseIds = sale.SaleDetails.Select(sd => sd.WarehouseId).Distinct().ToList();
            var warehouses = await _context.Warehouses
                .Where(w => warehouseIds.Contains(w.Id))
                .Select(w => new { w.Id, w.Name })
                .ToListAsync();

            return new SaleViewModel
            {
                SaleId = sale.SaleId,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                DocumentNumber = sale.DocumentNumber,
                IsPaid = sale.IsPaid,
                CustomerId = sale.CustomerId,
                CustomerName = sale.Customer?.Name,
                SaleDetails = sale.SaleDetails.Select(sd => new SaleDetailViewModel
                {
                    ProductId = sd.ProductId,
                    ProductName = products.FirstOrDefault(p => p.Id == sd.ProductId)?.Name ?? "Unknown",
                    WarehouseId = sd.WarehouseId,
                    WarehouseName = warehouses.FirstOrDefault(w => w.Id == sd.WarehouseId)?.Name ?? "Unknown",
                    Quantity = sd.Quantity,
                    Price = sd.Price
                }).ToList(),
                Payments = sale.Payments.Select(p => new PaymentViewModel
                {
                    PaymentId = p.PaymentId,
                    SaleId = p.SaleId,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    PaymentMethod = p.PaymentMethod
                }).ToList()
            };
        }

        public async Task<WorkflowResult> CreateSaleAsync(SaleViewModel model)
        {
            try
            {
                foreach (var detail in model.SaleDetails)
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
                    var workflowDataFirstRun = new SaleWorkflowData
                    {
                        SaleDate = model.SaleDate,
                        TotalAmount = model.TotalAmount,
                        SaleDetails = model.SaleDetails,
                        
                    };

                    var workflowInstanceFirstRun = await _workflow.StartWorkflow("NoStockSupplier", workflowDataFirstRun);
                    var resultFirstRun = await WaitForWorkflowToComplete(workflowInstanceFirstRun);
                    if (!resultFirstRun.IsSuccessful)
                    {
                        return new WorkflowResult
                        {
                            IsSuccessful = false,
                            ErrorMessage = resultFirstRun.ErrorMessage
                        };
                    }
                    if (stock.Quantity <= 0 || stock.Quantity < detail.Quantity)
                    {
                        return new WorkflowResult
                        {
                            IsSuccessful = false,
                            ErrorMessage = $"Insufficient stock for Product: {detail.ProductName} in Warehouse: {detail.WarehouseName}"
                        };
                    }
                }

                var warehouseId = model.SaleDetails.First().WarehouseId;
                var salesInWarehouse = await _context.Sales
                    .Where(s => s.SaleDetails.Any(sd => sd.WarehouseId == warehouseId))
                    .CountAsync();

                int nextSaleNumber = salesInWarehouse + 1;
                var warehouseCode = warehouseId.ToString("D3");

                var documentNumber = $"{nextSaleNumber}-{warehouseCode}-S1AP";

                var sale = new Sale
                {
                    SaleDate = model.SaleDate,
                    TotalAmount = model.TotalAmount,
                    DocumentNumber = documentNumber,
                    IsPaid = model.IsPaid,
                    CustomerId = model.CustomerId,
                    SaleDetails = model.SaleDetails.Select(sd => new SaleDetail
                    {
                        ProductId = sd.ProductId,
                        WarehouseId = sd.WarehouseId,
                        Quantity = sd.Quantity,
                        Price = sd.Price
                    }).ToList()
                };
                var customer = await _context.Customers.FindAsync(model.CustomerId);
                string customerName = customer?.Name ?? "Unknown Customer";

                var workflowData = new SaleWorkflowData
                {
                    SaleDate = model.SaleDate,
                    TotalAmount = model.TotalAmount,
                    SaleDetails = model.SaleDetails,
                    DocumentNumber = documentNumber,
                    CustomerName = customerName
                };

                var workflowInstance = await _workflow.StartWorkflow("SaleWorkflow", workflowData);

                var result = await WaitForWorkflowToComplete(workflowInstance);
                if (!result.IsSuccessful)
                {
                    return new WorkflowResult
                    {
                        IsSuccessful = false,
                        ErrorMessage = result.ErrorMessage
                    };
                }

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();
                if (model.IsPaid)
                {
                    var payment = new Payment
                    {
                        SaleId = sale.SaleId,
                        PaymentDate = DateTime.Now,
                        Amount = sale.TotalAmount,
                        PaymentMethod = "Cash", 
                        DocumentNumber = $"PAYED-{DateTime.Now.Ticks}",
                        CustomerId = model.CustomerId,
                        IsPaid = true
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync(); 

                    
                    sale.PaymentId = payment.PaymentId;
                    sale.PaymentDocumentNumber = payment.DocumentNumber;

                    _context.Sales.Update(sale); 
                    await _context.SaveChangesAsync(); 
                }
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
                    ErrorMessage = "Error processing sale: " + ex.Message
                };
            }
        }

        public async Task UpdateSaleAsync(SaleViewModel saleViewModel)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .FirstOrDefaultAsync(s => s.SaleId == saleViewModel.SaleId);

            if (sale == null)
            {
                throw new ArgumentException("Sale not found.");
            }

            var oldSaleDetails = sale.SaleDetails.ToList();

            var productIds = oldSaleDetails.Select(sd => sd.ProductId).Union(saleViewModel.SaleDetails.Select(sd => sd.ProductId)).Distinct().ToList();
            var warehouseIds = oldSaleDetails.Select(sd => sd.WarehouseId).Union(saleViewModel.SaleDetails.Select(sd => sd.WarehouseId)).Distinct().ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Name);

            var warehouses = await _context.Warehouses
                .Where(w => warehouseIds.Contains(w.Id))
                .ToDictionaryAsync(w => w.Id, w => w.Name);

            var documentNumber = sale.DocumentNumber;
            var customer = await _context.Customers.FindAsync(sale.CustomerId);
            string customerName = customer?.Name ?? "Unknown Customer";
            var workflowData = new SaleWorkflowData
            {
                OldSaleDetails = oldSaleDetails.Select(sd => new SaleDetailViewModel
                {
                    ProductId = sd.ProductId,
                    ProductName = products.ContainsKey(sd.ProductId) ? products[sd.ProductId] : "Unknown",
                    WarehouseId = sd.WarehouseId,
                    WarehouseName = warehouses.ContainsKey(sd.WarehouseId) ? warehouses[sd.WarehouseId] : "Unknown",
                    Quantity = sd.Quantity,
                    Price = sd.Price
                }).ToList(),
                NewSaleDetails = saleViewModel.SaleDetails.Select(sd => new SaleDetailViewModel
                {
                    ProductId = sd.ProductId,
                    ProductName = products.ContainsKey(sd.ProductId) ? products[sd.ProductId] : "Unknown",
                    WarehouseId = sd.WarehouseId,
                    WarehouseName = warehouses.ContainsKey(sd.WarehouseId) ? warehouses[sd.WarehouseId] : "Unknown",
                    Quantity = sd.Quantity,
                    Price = sd.Price
                }).ToList(),
                DocumentNumber = documentNumber,
                CustomerName = customerName
            };

            var workflowInstance = await _workflow.StartWorkflow("UpdateSaleWorkflow", workflowData);
            var result = await WaitForWorkflowToComplete(workflowInstance);

            if (!result.IsSuccessful)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }

            sale.SaleDate = saleViewModel.SaleDate;
            sale.TotalAmount = 0;
            sale.IsPaid = saleViewModel.IsPaid;
            sale.CustomerId = saleViewModel.CustomerId;

            foreach (var oldDetail in oldSaleDetails)
            {
                if (!saleViewModel.SaleDetails.Any(sd => sd.ProductId == oldDetail.ProductId && sd.WarehouseId == oldDetail.WarehouseId))
                {
                    _context.SaleDetails.Remove(oldDetail);
                }
            }

            foreach (var detail in saleViewModel.SaleDetails)
            {
                var existingDetail = oldSaleDetails.FirstOrDefault(sd => sd.ProductId == detail.ProductId && sd.WarehouseId == detail.WarehouseId);

                if (existingDetail != null)
                {
                    existingDetail.Quantity = detail.Quantity;
                    existingDetail.Price = detail.Price;
                }
                else
                {
                    var saleDetail = new SaleDetail
                    {
                        ProductId = detail.ProductId,
                        WarehouseId = detail.WarehouseId,
                        Quantity = detail.Quantity,
                        Price = detail.Price
                    };
                    sale.SaleDetails.Add(saleDetail);
                }

                sale.TotalAmount += detail.Quantity * detail.Price;
            }

            _context.Update(sale);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSaleAsync(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale == null) return;

            var workflowData = new SaleWorkflowData
            {
                OldSaleDetails = sale.SaleDetails.Select(sd => new SaleDetailViewModel
                {
                    ProductId = sd.ProductId,
                    WarehouseId = sd.WarehouseId,
                    Quantity = sd.Quantity,
                    Price = sd.Price
                }).ToList()
            };

            var workflowInstance = await _workflow.StartWorkflow("DeleteSaleWorkflow", workflowData);
            var result = await WaitForWorkflowToComplete(workflowInstance);

            if (!result.IsSuccessful)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }

            _context.Sales.Remove(sale);
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

    public class WorkflowResult
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
    }
}
