using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using WorkflowCore.Interface;

namespace TemaEDiplomesUBT.Workflow
{
    public class StockTransferWorkflow : IWorkflow<StockTransferData>
    {
        public string Id => "StockTransferWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<StockTransferData> builder)
        {
            builder
                .StartWith<DeductStockStep>()
                    .Input(step => step.SourceWarehouseId, (data, context) => data.SourceWarehouseId)
                    .Input(step => step.ProductId, (data, context) => data.ProductId)
                    .Input(step => step.Quantity, (data, context) => data.Quantity)
                .Then<AddStockToDestinationStep>()
                    .Input(step => step.DestinationWarehouseId, (data, context) => data.DestinationWarehouseId)
                    .Input(step => step.ProductId, (data, context) => data.ProductId)
                    .Input(step => step.Quantity, (data, context) => data.Quantity);
        }
    }
}
