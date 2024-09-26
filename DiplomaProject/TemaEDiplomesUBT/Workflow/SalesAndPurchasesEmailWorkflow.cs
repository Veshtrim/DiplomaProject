using WorkflowCore.Interface;
using WorkflowCore.Models;
using TemaEDiplomesUBT.Workflow.WorkflowSteps;

namespace TemaEDiplomesUBT.Workflow
{
    public class SalesAndPurchasesEmailWorkflow : IWorkflow
    {
        public string Id => "SalesAndPurchasesEmailWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder
                .StartWith<SalesAndPurchasesEmail>() 
                .Then(context => ExecutionResult.Next()); 
        }
    }
}
