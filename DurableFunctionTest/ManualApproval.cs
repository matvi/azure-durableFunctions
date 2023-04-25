
using DurableFunctionTest.Activities;
using DurableFunctionTest.Constants;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DurableFunctionTest
{
    public class ManualApproval
    {
        const string ApprovalEvent = "ApprovalEvent";
        [FunctionName(nameof(ManuallyApproveRecognition))]
        public async Task<bool> ManuallyApproveRecognition(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            logger.LogError("*************Entering the manual approve recognition");
            var speedViolation = context.GetInput<SpeedViolation>();

            //setting up the 60 minutes timer that this function will wait for human to respond
            logger.LogError("*************Setting Timer to wait for Humnan approval");
            using var timeOutCts = new CancellationTokenSource();
            var expiration = context.CurrentUtcDateTime.AddSeconds(10);
            var timeoutTask = context.CreateTimer(expiration, timeOutCts.Token);

            //wait for slack response
            //need to add the contextId
            var approvalRequest = new ApprovalRequest
            {
                DurableContextId = context.InstanceId
            };

            //calling the slack api 
            logger.LogError("************* Calling Slack API to send information to human");
            await context.CallActivityAsync(nameof(Activity1.SendApproval), approvalRequest);

            var approvalResponse = context.WaitForExternalEvent<bool>(CustomConstants.ApprovalEvent);

            
            logger.LogError($"*************current date on context : {context.CurrentUtcDateTime}");

            //wait for the timer and send approval until one finishes first
            logger.LogError("*************Waiting for timer to finished or Human to respond");
            var winner = await Task.WhenAny(timeoutTask, approvalResponse);
            

            if (!timeoutTask.IsCompleted)
            {
                logger.LogError("*************Approved by manual user..cancelling timer");
                timeOutCts.Cancel();
            }
            else
            {
                logger.LogError("################ Time out reached");
                return false;
            }

            logger.LogError($"################ User response {approvalResponse.Result}");

            if (WasManualApprovedAndSuccess(approvalResponse, winner))
            {
                logger.LogError($"################ Storing response in Database");
                await context.CallActivityAsync(nameof(StoreActivity.StoreSpeedViolation), speedViolation);
            }

            return true;
        }

        private static bool WasManualApprovedAndSuccess(Task<bool> approvalResponse, Task winner)
        {
            return winner == approvalResponse && approvalResponse.Result;
        }
    }
}
