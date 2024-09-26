using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using WorkflowCore.Interface;

namespace TemaEDiplomesUBT.Workflow
{
    public class SaleWorkflow : IWorkflow<SaleWorkflowData>
    {
        public string Id => "SaleWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<SaleWorkflowData> builder)
        {
            builder
                .StartWith<UpdateStockForSaleStep>()
                    .Input(step => step.SaleDetails, data => data.SaleDetails)
                 .Then<GeneratePdfReceiptStep>()
                    .Input(step => step.DocumentNumber, data => data.DocumentNumber) 
                    .Input(step => step.SaleDetails, data => data.SaleDetails)
                    .Input(step => step.CustomerName, data => data.CustomerName)
                .EndWorkflow();
        }
    }



}
