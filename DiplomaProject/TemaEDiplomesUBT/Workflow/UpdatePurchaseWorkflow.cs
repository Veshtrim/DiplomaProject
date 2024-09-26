using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using WorkflowCore.Interface;

namespace TemaEDiplomesUBT.Workflow
{
    public class UpdatePurchaseWorkflow : IWorkflow<PurchaseWorkflowData>
    {
        public string Id => "UpdatePurchaseWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<PurchaseWorkflowData> builder)
        {
            builder
                .StartWith<AdjustStockForUpdatedPurchaseStep>()
                    .Input(step => step.OldPurchaseDetails, data => data.OldPurchase)
                    .Input(step => step.NewPurchaseDetails, data => data.NewPurchase)
                    .Then<GeneratePdfReceiptPurchaseStep>()
                    .Input(step => step.DocumentNumber, data => data.PurchaseDocumentNumber)
                    .Input(step => step.PurchaseDetails, data => data.NewPurchase)
                    .Input(step => step.SupplierName, data => data.SupplierName)
                    .Input(step => step.IsUpdate, data => true)
                    .EndWorkflow();
        }
    }
}
