using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Services.IServices;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using WorkflowCore.Interface;
using Microsoft.AspNetCore.Mvc;

namespace TemaEDiplomesUBT.Services
{
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkflowHost _workflowHost;

        public StockService(ApplicationDbContext context, IWorkflowHost workflowHost)
        {
            _context = context;
            _workflowHost = workflowHost;
        }

        public IEnumerable<StockViewModel> GetStocks()
        {
            return _context.Stocks.Select(s => new StockViewModel
            {
                ProductId = s.ProductId,
                ProductName = s.Product.Name,
                WarehouseId = s.WarehouseId,
                WarehouseName = s.Warehouse.Name,
                Quantity = s.Quantity
            }).ToList();
        }

        public async Task AdjustStock(int productId, int warehouseId, int quantity)
        {
            var data = new StockAdjustmentData
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                Quantity = quantity
            };

          
            var workflowInstance = await _workflowHost.StartWorkflow("StockAdjustmentWorkflow", data);

            
            await WaitForWorkflowToComplete(workflowInstance);
        }

        public async Task TransferStock(int sourceWarehouseId, int destinationWarehouseId, int productId, int quantity)
        {
            var data = new StockTransferData
            {
                SourceWarehouseId = sourceWarehouseId,
                DestinationWarehouseId = destinationWarehouseId,
                ProductId = productId,
                Quantity = quantity
            };

            var workflowInstance = await _workflowHost.StartWorkflow("StockTransferWorkflow", data);

           
            await WaitForWorkflowToComplete(workflowInstance);
        }
        public async Task WaitForWorkflowToComplete(string workflowId)
        {
            while (true)
            {
              
                var workflowInstance = await _workflowHost.PersistenceStore.GetWorkflowInstance(workflowId);

                if (workflowInstance.Status == WorkflowCore.Models.WorkflowStatus.Complete ||
                    workflowInstance.Status == WorkflowCore.Models.WorkflowStatus.Terminated)
                {
                   
                    break;
                }

             
                await Task.Delay(1000); 
            }
        }
    }

}
