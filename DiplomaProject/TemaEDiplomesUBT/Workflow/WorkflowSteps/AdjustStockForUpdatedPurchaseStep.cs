using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models.ViewModels;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class AdjustStockForUpdatedPurchaseStep : StepBody
    {
        private readonly ApplicationDbContext _dbContext;

        public AdjustStockForUpdatedPurchaseStep(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<PurchaseDetailViewModel> OldPurchaseDetails { get; set; }
        public List<PurchaseDetailViewModel> NewPurchaseDetails { get; set; }
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            
            var unchangedOldDetails = OldPurchaseDetails.Select(old => new PurchaseDetailViewModel
            {
                ProductId = old.ProductId,
                WarehouseId = old.WarehouseId,
                Quantity = old.Quantity,
                PurchasePrice = old.PurchasePrice
            }).ToList();

           
            foreach (var oldDetail in unchangedOldDetails)
            {
                var newDetail = NewPurchaseDetails.FirstOrDefault(n => n.ProductId == oldDetail.ProductId && n.WarehouseId == oldDetail.WarehouseId);
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.ProductId == oldDetail.ProductId && s.WarehouseId == oldDetail.WarehouseId);

                if (stock != null)
                {
                    if (newDetail == null)
                    {
                      
                        stock.Quantity -= oldDetail.Quantity;
                    }
                    else
                    {
                      
                        var quantityDifference = oldDetail.Quantity - newDetail.Quantity;
                        if (quantityDifference > 0)
                        {
                            stock.Quantity -= quantityDifference;
                        }
                    }
                }
            }

          
            foreach (var newDetail in NewPurchaseDetails)
            {
                var oldDetail = unchangedOldDetails.FirstOrDefault(o => o.ProductId == newDetail.ProductId && o.WarehouseId == newDetail.WarehouseId);
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.ProductId == newDetail.ProductId && s.WarehouseId == newDetail.WarehouseId);

                if (stock != null)
                {
                    if (oldDetail == null)
                    {
                       
                        stock.Quantity += newDetail.Quantity;
                    }
                    else
                    {
                      
                        var quantityDifference = newDetail.Quantity - oldDetail.Quantity;
                        if (quantityDifference > 0)
                        {
                            
                            stock.Quantity += quantityDifference;
                        }
                    }
                }
            }

            _dbContext.SaveChanges();
            return ExecutionResult.Next();
        }
    }
}
