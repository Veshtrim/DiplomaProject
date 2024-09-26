using TemaEDiplomesUBT.Workflow.WorkflowSteps;
using WorkflowCore.Interface;

namespace TemaEDiplomesUBT.Workflow
{
    public class DayOffWorkflow : IWorkflow
    {
        public string Id => "DayOffWorkflow";
        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder
                .StartWith<SendSmsStep>()
                    .Input(step => step.PhoneNumber, context => "+38649137277")  
                    .Input(step => step.Message, context => "Reminder: Tomorrow is a day off!")  
                .Then(context => Console.WriteLine("SMS sent successfully for the day off."));
        }
    }
}
