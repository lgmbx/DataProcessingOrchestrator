using DataProcessingOrchestrator;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Company.Function
{
    public static class DurableFunctionsOrchestration
    {
        [FunctionName("DurableFunctionsOrchestration_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string body = await req.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<InputOrderModel>(body);

            string instanceId = await starter.StartNewAsync("DurableFunctionsOrchestration", data);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }


        [FunctionName("DurableFunctionsOrchestration")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var inputData = context.GetInput<InputOrderModel>();

            if (!ValidateOrder(inputData))
            {
                return "Order is not valid";
            }

            var processingData = new ProcessingData(inputData);

            foreach (var step in ProcessingSteps.Steps)
            {
                processingData.Step = step;
                var result = await context.CallActivityAsync<ProcessingData>(nameof(DataProcessingActivity), processingData);
                processingData = result;
            }

            return processingData.ToString();
        }

        [FunctionName("DataProcessingActivity")]
        public static ProcessingData DataProcessingActivity(
            [ActivityTrigger] ProcessingData processingData,
            ILogger log
        )
        {
            switch(processingData.Step)
            {
                case ProcessingStepEnum.OrderRequest:
                    log.LogInformation($"Order request received: {processingData.ProductName} - {processingData.Quantity} - {processingData.UnitPrice}");
                    processingData.OrderNumber = new Random().Next(1000, 9999);
                    break;

                case ProcessingStepEnum.Payment:
                    log.LogInformation($"Payment received for order {processingData.OrderNumber}");
                    processingData.TotalPaid = processingData.Quantity * processingData.UnitPrice;
                    break;

                case ProcessingStepEnum.Approval:
                    log.LogInformation($"Approval received for order {processingData.OrderNumber}");
                    if(processingData.TotalPaid > 0)
                        processingData.IsApproved = true;
                    break;

                case ProcessingStepEnum.ProcessOrder:
                    log.LogInformation($"Order {processingData.OrderNumber} is being processed");
                    processingData.IsOrderProcessed = true;
                    processingData.LastUpdate = DateTime.Now;
                    break;

                case ProcessingStepEnum.SendOrder:
                    log.LogInformation($"Order {processingData.OrderNumber} is being sent");
                    processingData.IsOrderSent = true;
                    processingData.LastUpdate = DateTime.Now;
                    break;
            }

            return processingData;
        }


        public static bool ValidateOrder(InputOrderModel inputData)
        {
            if (string.IsNullOrEmpty(inputData.ProductName))
            {
                return false;
            }
            if (inputData.Quantity <= 0)
            {
                return false;
            }
            if (inputData.UnitPrice <= 0)
            {
                return false;
            }

            return true;
        }
    }
}