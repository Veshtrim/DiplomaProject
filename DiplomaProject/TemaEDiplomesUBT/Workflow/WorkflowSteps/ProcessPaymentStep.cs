using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using Microsoft.EntityFrameworkCore;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class ProcessPaymentStep : StepBodyAsync
    {
        private readonly ApplicationDbContext _context;

        public ProcessPaymentStep(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var workflowData = context.Workflow.Data as PaymentWorkflowData;
            var sale = await _context.Sales.FindAsync(workflowData.SaleId);
            if (sale == null)
            {
                throw new InvalidOperationException($"Sale with ID {workflowData.SaleId} does not exist.");
            }
            workflowData.PaymentDocumentNumber = $"PAYED-{DateTime.Now.Ticks}";

           
            var totalAmount = await _context.Sales
                .Where(s => workflowData.SaleIds.Contains(s.SaleId))
                .SumAsync(s => s.TotalAmount);

            var payment = new Payment
            {
                SaleId = workflowData.SaleId,
                CustomerId = workflowData.CustomerId,
                Amount = totalAmount,
                PaymentDate = DateTime.Now,
                PaymentMethod = "To set up",
                DocumentNumber = workflowData.PaymentDocumentNumber,
                IsPaid = true
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

         
            workflowData.PaymentDocumentNumber = payment.DocumentNumber;

            return ExecutionResult.Next();
        }
    }
}
