using Microsoft.Extensions.Hosting;
using WorkflowCore.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TemaEDiplomesUBT.Services
{
    public class ScheduledEmailService : BackgroundService
    {
        private readonly IWorkflowHost _workflowHost;

        public ScheduledEmailService(IWorkflowHost workflowHost)
        {
            _workflowHost = workflowHost;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = DateTime.Today.AddHours(24).AddMinutes(00);  

             
                if (nextRun <= now)
                {
                    nextRun = nextRun.AddDays(1);  
                }

                var delay = nextRun - now;  

               
                await Task.Delay((int)delay.TotalMilliseconds);  

              
                _workflowHost.StartWorkflow("SalesAndPurchasesEmailWorkflow");
            }
        }
    }
}
