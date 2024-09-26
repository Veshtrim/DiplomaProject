using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TemaEDiplomesUBT.Services.IServices;
using WorkflowCore.Interface;

public class ScheduledDayOffCheckService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IWorkflowHost _workflowHost;

    public ScheduledDayOffCheckService(IServiceScopeFactory serviceScopeFactory, IWorkflowHost workflowHost)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _workflowHost = workflowHost;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = DateTime.Today.AddHours(20).AddMinutes(6);  

            if (nextRun <= now)
            {
                nextRun = nextRun.AddDays(1); 
            }

            var delay = nextRun - now;
            await Task.Delay((int)delay.TotalMilliseconds, stoppingToken); 

            await CheckAndSendSmsForTomorrow(stoppingToken); 
        }
    }

    private async Task CheckAndSendSmsForTomorrow(CancellationToken stoppingToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dayOffService = scope.ServiceProvider.GetRequiredService<IDayOffService>();

            var dayOff = await dayOffService.GetDayOffForTomorrowAsync();

            if (dayOff != null && !stoppingToken.IsCancellationRequested)
            {
                
                _workflowHost.StartWorkflow("DayOffWorkflow", null, new
                {
                    PhoneNumber = "+38649137277",  
                    Message = $"Reminder: Tomorrow ({dayOff.DayOffDate:yyyy-MM-dd}) is a day off! Reason:{dayOff.Reason}"
                });
            }
        }
    }
}
