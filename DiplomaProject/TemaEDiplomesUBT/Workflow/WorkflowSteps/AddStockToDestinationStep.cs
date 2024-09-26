using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class AddStockToDestinationStep : StepBody
    {
        public int DestinationWarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        private readonly ApplicationDbContext _dbContext;

        public AddStockToDestinationStep(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var stock = _dbContext.Stocks.FirstOrDefault(s => s.ProductId == ProductId && s.WarehouseId == DestinationWarehouseId);
            if (stock != null)
            {
                stock.Quantity += Quantity;
            }
            else
            {
                _dbContext.Stocks.Add(new Stock
                {
                    ProductId = ProductId,
                    WarehouseId = DestinationWarehouseId,
                    Quantity = Quantity
                });
            }
            _dbContext.SaveChanges();
            return ExecutionResult.Next();
        }
    }

}
