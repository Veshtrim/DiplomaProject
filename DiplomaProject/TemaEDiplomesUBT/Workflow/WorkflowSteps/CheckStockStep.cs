using Microsoft.AspNetCore.Identity.UI.Services;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class CheckStockStep : StepBody
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;

        public CheckStockStep(ApplicationDbContext dbContext, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var stock = _dbContext.Stocks
                .FirstOrDefault(s => s.ProductId == ProductId && s.WarehouseId == WarehouseId);

            if (stock == null)
            {
                _dbContext.Stocks.Add(new Stock
                {
                    ProductId = ProductId,
                    WarehouseId = WarehouseId,
                    Quantity = 0
                });
                _dbContext.SaveChanges();
            } else if (stock.Quantity <= 0) 
            {
              
                var suppliers = _dbContext.Suppliers.ToList();
                foreach (var supplier in suppliers)
                {
                     _emailSender.SendEmailAsync(supplier.Email, "Informations",
                        $"Is there any avaliabe for the ");
                }
            }
             
            return ExecutionResult.Next();
        }
    }

}
