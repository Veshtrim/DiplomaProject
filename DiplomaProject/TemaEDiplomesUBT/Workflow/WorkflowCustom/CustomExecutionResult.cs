using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowCustom
{
    public class CustomExecutionResult : ExecutionResult
    {
        public string CustomStatus { get; set; }

        public static CustomExecutionResult Terminate(string message)
        {
            return new CustomExecutionResult
            {
                Proceed = false,
                CustomStatus = "Terminated",
                PersistenceData = message
            };
        }
    }

}
