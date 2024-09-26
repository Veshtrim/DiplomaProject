using WorkflowCore.Interface;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;

namespace TemaEDiplomesUBT.Workflow
{
    public class DebtWorkflow : IWorkflow<DebtPaymentWorkflowData>
    {
        public string Id => "DebtWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<DebtPaymentWorkflowData> builder)
        {
            builder
                .StartWith<ProcessDebtStep>()
                .Then<UpdatePurchaseInDebt>();
        }
    }
}
