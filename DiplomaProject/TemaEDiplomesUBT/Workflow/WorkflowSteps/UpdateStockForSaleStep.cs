using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models.ViewModels;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class UpdateStockForSaleStep : StepBody
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;
        public UpdateStockForSaleStep(ApplicationDbContext dbContext , IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        public List<SaleDetailViewModel> SaleDetails { get; set; }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            foreach (var detail in SaleDetails)
            {
                var stock = _dbContext.Stocks
                    .FirstOrDefault(s => s.ProductId == detail.ProductId && s.WarehouseId == detail.WarehouseId);

                if (stock != null && stock.Quantity >= detail.Quantity)
                {
                    stock.Quantity -= detail.Quantity;
                    _dbContext.SaveChanges();
                }
                else if (stock.Quantity <= 0)
                {

                    var suppliers = _dbContext.Suppliers.ToList();
                    foreach (var supplier in suppliers)
                    {
                        _emailSender.SendEmailAsync(supplier.Email, "Informations",
                           $"Is there any avaliabe for the ");
                    }
                }


            }

            return ExecutionResult.Next(); 
        }
    }
}
