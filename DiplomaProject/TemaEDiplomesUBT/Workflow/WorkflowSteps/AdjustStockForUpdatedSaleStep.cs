using TemaEDiplomesUBT.Data;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using System.Linq;
using System.Collections.Generic;
using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class AdjustStockForUpdatedSaleStep : StepBody
    {
        private readonly ApplicationDbContext _dbContext;

        public AdjustStockForUpdatedSaleStep(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SaleDetailViewModel> OldSaleDetails { get; set; }
        public List<SaleDetailViewModel> NewSaleDetails { get; set; }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            // Ensure old details are not modified
            var unchangedOldDetails = OldSaleDetails.Select(old => new SaleDetailViewModel
            {
                ProductId = old.ProductId,
                WarehouseId = old.WarehouseId,
                Quantity = old.Quantity,
                Price = old.Price
            }).ToList();

            // Restore stock for deleted or decreased details
            foreach (var oldDetail in unchangedOldDetails)
            {
                var newDetail = NewSaleDetails.FirstOrDefault(n => n.ProductId == oldDetail.ProductId && n.WarehouseId == oldDetail.WarehouseId);
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.ProductId == oldDetail.ProductId && s.WarehouseId == oldDetail.WarehouseId);

                if (stock != null)
                {
                    if (newDetail == null)
                    {
                        // The detail was removed, restore full quantity to stock
                        stock.Quantity += oldDetail.Quantity;
                    }
                    else
                    {
                        // The detail exists but the quantity might have changed
                        var quantityDifference = oldDetail.Quantity - newDetail.Quantity;
                        if (quantityDifference > 0)
                        {
                            // Quantity decreased, restore the difference to stock
                            stock.Quantity += quantityDifference;
                        }
                    }
                }
            }

            // Adjust stock based on new or increased details
            foreach (var newDetail in NewSaleDetails)
            {
                var oldDetail = unchangedOldDetails.FirstOrDefault(o => o.ProductId == newDetail.ProductId && o.WarehouseId == newDetail.WarehouseId);
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.ProductId == newDetail.ProductId && s.WarehouseId == newDetail.WarehouseId);

                if (stock != null)
                {
                    if (oldDetail == null)
                    {
                        // New detail added, decrease stock by the quantity
                        stock.Quantity -= newDetail.Quantity;
                    }
                    else
                    {
                        // The detail exists but the quantity might have increased
                        var quantityDifference = newDetail.Quantity - oldDetail.Quantity;
                        if (quantityDifference > 0)
                        {
                            // Quantity increased, decrease the stock by the difference
                            stock.Quantity -= quantityDifference;
                        }
                    }
                }
            }

            _dbContext.SaveChanges();
            return ExecutionResult.Next();
        }
    }
}
