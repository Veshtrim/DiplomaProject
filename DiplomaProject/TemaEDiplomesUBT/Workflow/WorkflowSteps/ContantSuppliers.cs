using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models.ViewModels;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class ContantSuppliers : StepBody
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        public ContantSuppliers(ApplicationDbContext context,IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }
        public List<SaleDetailViewModel> SaleDetails { get; set; }
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            foreach(var detail in SaleDetails)
            {
                var stock = _context.Stocks.FirstOrDefault(s => s.WarehouseId == detail.WarehouseId && s.ProductId == detail.ProductId);
                if (stock.Quantity <= 0)
                {

                    var suppliers = _context.Suppliers.ToList();
                    foreach (var supplier in suppliers)
                    {
                        _emailSender.SendEmailAsync(supplier.Email, "Informations",
                           $"Is there any avaliabe for the {detail.ProductName}");
                    }
                }
            }
            return ExecutionResult.Next();
        }
    }
}
