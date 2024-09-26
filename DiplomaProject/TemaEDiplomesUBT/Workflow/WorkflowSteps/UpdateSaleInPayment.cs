using Microsoft.EntityFrameworkCore;
using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class UpdateSaleInPayment : StepBodyAsync
    {
        private readonly ApplicationDbContext _context;

        public UpdateSaleInPayment(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var workflowData = context.Workflow.Data as PaymentWorkflowData;

           
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.DocumentNumber == workflowData.PaymentDocumentNumber);

      
            var sales = await _context.Sales
                .Where(s => workflowData.SaleIds.Contains(s.SaleId))
                .ToListAsync();

            foreach (var sale in sales)
            {
                sale.IsPaid = true;
                sale.PaymentId = payment.PaymentId;
                sale.PaymentDocumentNumber = payment.DocumentNumber;

                _context.Sales.Update(sale);
            }

            await _context.SaveChangesAsync();

            return ExecutionResult.Next();
        }
    }
}
