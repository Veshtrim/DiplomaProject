using Microsoft.Extensions.Options;
using System.Net.Http;
using TemaEDiplomesUBT.Models;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace TemaEDiplomesUBT.Workflow.WorkflowSteps
{
    public class SendSmsStep : StepBodyAsync
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly SendBerrySettings _settings;

        public SendSmsStep(IHttpClientFactory httpClient, IOptions<SendBerrySettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public string PhoneNumber { get; set; }
        public string Message { get; set; }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var httpClient = _httpClient.CreateClient();
            var apiUrl = $"{_settings.BaseUrl}?" +
                         $"key={_settings.ApiKey}" +
                         $"&name={_settings.AccessName}" +
                         $"&password={_settings.AccessPassword}" +
                         $"&to[]={PhoneNumber}" +
                         $"&from={_settings.SenderName}" +
                         $"&content={Uri.EscapeDataString(Message)}";


            var response = await httpClient.GetAsync(apiUrl);


            if (response.IsSuccessStatusCode)
            {
                return ExecutionResult.Next();
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to send SMS: {responseContent}");
                return ExecutionResult.Persist(null);  
            }
        }
    }
}
