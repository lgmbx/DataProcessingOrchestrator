using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class DurableFunctionsOrchestrationCSharp1
    {
        [FunctionName("DurableFunctionsOrchestrationCSharp1")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            string inputData = context.GetInput<string>();
            var processingSteps = new List<string>
            {
                "CleanseData",
                "TransformData",
            };

            var output = new List<string>();
            foreach (var step in processingSteps)
            {
                output.Add(await context.CallActivityAsync<string>(nameof(DataProcessingActivity), step + inputData));
            }

            return output;
        }

        [FunctionName("DataProcessingActivity")]
        public static string DataProcessingActivity([ActivityTrigger] string inputData, ILogger log)
        {
            switch (inputData)
            {
                case "CleanseData":
                    // Replace with actual cleansing logic
                    log.LogInformation("Data cleansing completed.");
                    break;
                case "TransformData":
                    // Replace with actual transformation logic
                    log.LogInformation("Data transformation completed.");
                    break;
                default:
                    log.LogError($"Unknown processing step: {inputData}");
                    break;
            }

            return inputData;
        }

        [FunctionName("DurableFunctionsOrchestrationCSharp1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("DurableFunctionsOrchestrationCSharp1", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}