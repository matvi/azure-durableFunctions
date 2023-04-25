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
using System.Net.Http;

namespace DurableFunctionTest
{
    public class Function1
    {
        private const double speedThrehold = 0.8;

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogError("*************C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var speedViolation = JsonConvert.DeserializeObject<SpeedViolation>(requestBody);
            
            if(speedViolation.AccuracyRecognition < speedThrehold)
            {
                log.LogError($"*************Camara could not reach the accuracy required for automatic recognition {speedViolation.AccuracyRecognition}. Sending to human recognition.");
                //send plate for human recognition

                //calls the durable function
                string instanceId = await starter.StartNewAsync(nameof(ManualApproval.ManuallyApproveRecognition),speedViolation);
                //returns status object
                return starter.CreateCheckStatusResponse(req, instanceId);
            }

            await StoreSpeedViolation(speedViolation, log);
            log.LogError($"*************Returning response {speedViolation.AccuracyRecognition}. Sending to human recognition.");

            return new OkObjectResult(speedViolation);
        }

        public Task StoreSpeedViolation(SpeedViolation speedViolation, ILogger logger)
        {
            logger.LogError($"speed violation recognition saved in database");
            return Task.CompletedTask;
        }
    }
}
