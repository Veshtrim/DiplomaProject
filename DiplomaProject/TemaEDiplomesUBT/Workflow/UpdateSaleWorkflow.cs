using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using WorkflowCore.Interface;

namespace TemaEDiplomesUBT.Workflow
{
    public class UpdateSaleWorkflow : IWorkflow<SaleWorkflowData>
    {
        public string Id => "UpdateSaleWorkflow";
        public int Version => 1;
        public void Build(IWorkflowBuilder<SaleWorkflowData> builder)
        {
            builder
            .StartWith<AdjustStockForUpdatedSaleStep>()
                .Input(step => step.OldSaleDetails, data => data.OldSaleDetails)
                .Input(step => step.NewSaleDetails, data => data.NewSaleDetails)
                .Then<GeneratePdfReceiptStep>()
                .Input(step => step.DocumentNumber, data => data.DocumentNumber)
                .Input(step => step.SaleDetails, data => data.NewSaleDetails)
                .Input(step => step.CustomerName, data => data.CustomerName) 
                .Input(step => step.IsUpdate, data => true)
                .EndWorkflow();
        }

    }

}
