using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class ProcessDebtStep : StepBodyAsync
    {
        private readonly ApplicationDbContext _context;

        public ProcessDebtStep(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var workflowData = context.Workflow.Data as DebtPaymentWorkflowData;
            var purchase = await _context.Purchases.FindAsync(workflowData.PurchaseId);

            if (purchase == null)
            {
                throw new InvalidOperationException($"Purchase with ID {workflowData.PurchaseId} does not exist.");
            }

            workflowData.PaymentDocumentNumber = $"DEBT-{DateTime.Now.Ticks}";

            var totalAmount = await _context.Debts
                .Where(d => workflowData.PurchaseIds.Contains(d.PurchaseId))
                .SumAsync(d => d.Amount);

            var debt = new Debt
            {
                PurchaseId = workflowData.PurchaseId,
                SupplierId = workflowData.SupplierId,
                Amount = totalAmount,
                PaymentDate = DateTime.Now,
                PaymentMethod = "To set up",
                DocumentNumber = workflowData.PaymentDocumentNumber,
                IsPaid = true
            };

            _context.Debts.Add(debt);
            await _context.SaveChangesAsync();

            workflowData.PaymentDocumentNumber = debt.DocumentNumber;

            return ExecutionResult.Next();
        }
    }
}
