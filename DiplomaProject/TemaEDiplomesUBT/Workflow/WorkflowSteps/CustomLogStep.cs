using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class CustomLogStep : StepBody
    {
        public string Message { get; set; }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine(Message);
            return ExecutionResult.Next();
        }
    }
}
