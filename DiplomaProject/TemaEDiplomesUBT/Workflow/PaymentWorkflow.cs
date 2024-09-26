using WorkflowCore.Interface;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;

namespace TemaEDiplomesUBT.Workflow
{
    public class PaymentWorkflow : IWorkflow<PaymentWorkflowData>
    {
        public string Id => "PaymentWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<PaymentWorkflowData> builder)
        {
            builder
                .StartWith<ProcessPaymentStep>()
                .Then<UpdateSaleInPayment>();
        }
    }
}
