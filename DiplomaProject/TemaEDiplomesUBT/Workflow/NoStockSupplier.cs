using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using WorkflowCore.Interface;

namespace TemaEDiplomesUBT.Workflow
{
    public class NoStockSupplier : IWorkflow<SaleWorkflowData>
    {
        public string Id => "NoStockSupplier";
        public int Version => 1;

        public void Build(IWorkflowBuilder<SaleWorkflowData> builder)
        {
            builder
                .StartWith<ContantSuppliers>()
                    .Input(step => step.SaleDetails, data => data.SaleDetails)
                 
                .EndWorkflow();
        }
    }
}
