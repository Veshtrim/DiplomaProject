using TemaEDiplomesUBT.Workflow.WorkflowData;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using WorkflowCore.Interface;

namespace TemaEDiplomesUBT.Workflow
{
    public class StockAdjustmentWorkflow : IWorkflow<StockAdjustmentData>
    {
        public string Id => "StockAdjustmentWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<StockAdjustmentData> builder)
        {
            builder
                .StartWith<CheckStockStep>()
                .Input(step => step.ProductId, data => data.ProductId)
                .Input(step => step.WarehouseId, data => data.WarehouseId)
                .Input(step => step.Quantity, data => data.Quantity)
                .Then<CheckStockStep>()
                .Input(step => step.ProductId, data => data.ProductId)
                .Input(step => step.WarehouseId, data => data.WarehouseId)
                .Input(step => step.Quantity, data => data.Quantity)
                .Then<UpdateStockStep>()
                .Input(step => step.ProductId, data => data.ProductId)
                .Input(step => step.WarehouseId, data => data.WarehouseId)
                .Input(step => step.Quantity, data => data.Quantity);
        }
    }

}
