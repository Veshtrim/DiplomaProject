using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class HandleErrorStep : StepBody
    {
        public string ErrorMessage { get; set; }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
         
            Console.WriteLine($"Error encountered: {ErrorMessage}");

          

            return ExecutionResult.Persist(ErrorMessage); 
        }
    }
}
