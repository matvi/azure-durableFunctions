using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using DurableFunctionTest.Contracts;
using DurableFunctionTest.Constants;

namespace DurableFunctionTest.Functions
{
    public static class ManualResponseFunction
    {
        [FunctionName("ManualResponseFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [DurableClient]IDurableClient durableClient,
            ILogger log)
        {
            log.LogError("**********ManualHumanResponseFunction ##################");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var manualUserRequest = JsonConvert.DeserializeObject<ManualUserRequest>(requestBody);

            if(manualUserRequest == null || string.IsNullOrEmpty(manualUserRequest.DurableInstanceId))
            {
                log.LogError("**********Error on ManualHumanResponseFunction ##################");
                return new BadRequestObjectResult("Error on object");
            }

            log.LogError("********** Human send a response. Getting back to Durable fucntion ##################");
            await durableClient.RaiseEventAsync(manualUserRequest.DurableInstanceId, CustomConstants.ApprovalEvent, manualUserRequest.Approved);

            return new OkObjectResult(manualUserRequest);
        }
    }
}
