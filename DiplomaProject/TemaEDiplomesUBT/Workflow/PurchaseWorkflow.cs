using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using WorkflowCore.Interface;

namespace TemaEDiplomesUBT.Workflow
{
    public class PurchaseWorkflow : IWorkflow<PurchaseWorkflowData>
    {
        public string Id => "PurchaseWorkflow";

        public int Version => 1;

        public void Build(IWorkflowBuilder<PurchaseWorkflowData> builder)
        {
            builder
                .StartWith<UpdateStockForPurchaseStep>()
                    .Input(step => step.PurchaseDetails, data => data.PurchaseDetails)
                     .Then<GeneratePdfReceiptPurchaseStep>()
                    .Input(step => step.DocumentNumber, data => data.PurchaseDocumentNumber)
                    .Input(step => step.PurchaseDetails, data => data.PurchaseDetails)
                    .Input(step => step.SupplierName, data => data.SupplierName)
                .EndWorkflow();

        }
    }
}
