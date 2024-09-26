using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class UpdateStockForPurchaseStep : StepBody
    {
        private readonly ApplicationDbContext _context;
        public UpdateStockForPurchaseStep(ApplicationDbContext context) { 
           _context = context;
        }
        
        public List<PurchaseDetailViewModel> PurchaseDetails { get; set; }
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            foreach (var detail in PurchaseDetails)
            {
                var stock = _context.Stocks
                    .FirstOrDefault(s => s.ProductId == detail.ProductId && s.WarehouseId == detail.WarehouseId);

                if (stock != null && stock.Quantity >= detail.Quantity)
                {
                    stock.Quantity += detail.Quantity;
                    _context.SaveChanges();
                }

            }

            return ExecutionResult.Next();
        }
    }
}
