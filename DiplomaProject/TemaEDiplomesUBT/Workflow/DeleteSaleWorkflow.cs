using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using WorkflowCore.Interface;

namespace TemaEDiplomesUBT.Workflow
{
    public class DeleteSaleWorkflow : IWorkflow<SaleWorkflowData>
    {
        public string Id => "DeleteSaleWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<SaleWorkflowData> builder)
        {
            builder
                .StartWith<RestoreStockForDeletedSaleStep>() 
                    .Input(step => step.SaleDetails, data => data.OldSaleDetails); 
        }
    }
}
