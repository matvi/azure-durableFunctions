using DurableFunctionTest.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionTest.Activities
{
    public class StoreActivity
    {
        [FunctionName(nameof(StoreSpeedViolation))]
        public Task StoreSpeedViolation([ActivityTrigger]SpeedViolation speedViolation, ILogger logger)
        {
            logger.LogError($"speed violation recognition saved in database");
            return Task.CompletedTask;
        }
    }
}
