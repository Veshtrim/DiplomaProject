using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class UpdatePurchaseInDebt : StepBodyAsync
    {
        private readonly ApplicationDbContext _context;

        public UpdatePurchaseInDebt(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var workflowData = context.Workflow.Data as DebtPaymentWorkflowData;

          
            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.DocumentNumber == workflowData.PaymentDocumentNumber);

            if (debt == null)
            {
                throw new InvalidOperationException($"Debt with Document Number {workflowData.PaymentDocumentNumber} does not exist.");
            }

           
            var purchases = await _context.Purchases
                .Where(p => workflowData.PurchaseIds.Contains(p.PurchaseId))
                .ToListAsync();

          
           
            foreach (var purchase in purchases)
            {
                purchase.IsPaid = true;
                purchase.DebtId = debt.DebtId;
                purchase.DebtDocumentNumber = debt.DocumentNumber;

                _context.Purchases.Update(purchase);
            }


            await _context.SaveChangesAsync();

            return ExecutionResult.Next(); 
        }
    }
}
