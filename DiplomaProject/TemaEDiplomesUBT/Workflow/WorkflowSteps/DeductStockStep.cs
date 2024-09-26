using TemaEDiplomesUBT.Data;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class DeductStockStep : StepBody
    {
        public int SourceWarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        private readonly ApplicationDbContext _dbContext;

        public DeductStockStep(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var stock = _dbContext.Stocks.FirstOrDefault(s => s.ProductId == ProductId && s.WarehouseId == SourceWarehouseId);
            if (stock != null)
            {
                stock.Quantity -= Quantity;
                _dbContext.SaveChanges();
            }
            return ExecutionResult.Next();
        }
    }

}
