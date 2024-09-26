using TemaEDiplomesUBT.Data;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class UpdateStockStep : StepBody
    {
        private readonly ApplicationDbContext _dbContext;

        public UpdateStockStep(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var stock = _dbContext.Stocks
                .FirstOrDefault(s => s.ProductId == ProductId && s.WarehouseId == WarehouseId);

            if (stock != null)
            {
                stock.Quantity += Quantity;
                _dbContext.SaveChanges();
            }

            return ExecutionResult.Next();
        }
    }

}
