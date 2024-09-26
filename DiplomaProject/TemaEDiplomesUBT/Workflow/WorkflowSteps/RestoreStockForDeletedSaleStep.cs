using TemaEDiplomesUBT.Data;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using System.Linq;
using System.Collections.Generic;
using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class RestoreStockForDeletedSaleStep : StepBody
    {
        private readonly ApplicationDbContext _dbContext;

        public RestoreStockForDeletedSaleStep(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SaleDetailViewModel> SaleDetails { get; set; } 

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            foreach (var detail in SaleDetails)
            {
                var stock = _dbContext.Stocks.FirstOrDefault(s => s.ProductId == detail.ProductId && s.WarehouseId == detail.WarehouseId);

                if (stock != null)
                {
                   
                    stock.Quantity += detail.Quantity;
                }
            }

            _dbContext.SaveChanges();
            return ExecutionResult.Next(); 
        }
    }
}
