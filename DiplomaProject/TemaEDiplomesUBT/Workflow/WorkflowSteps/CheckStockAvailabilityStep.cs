using TemaEDiplomesUBT.Data;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class CheckStockAvailabilityStep : StepBody
    {
        private readonly ApplicationDbContext _dbContext;

        public CheckStockAvailabilityStep(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int SourceWarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var stock = _dbContext.Stocks.FirstOrDefault(s => s.ProductId == ProductId && s.WarehouseId == SourceWarehouseId);

            if (stock == null || stock.Quantity < Quantity)
            {
                throw new InvalidOperationException("Not enough stock available in the source warehouse.");
            }

            return ExecutionResult.Next();
        }
    }


}
