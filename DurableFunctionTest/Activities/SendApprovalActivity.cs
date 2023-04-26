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
    public class SendApprovalActivity
    {
        [FunctionName(nameof(SendApprovalActivity.SendApproval))]
        public Task SendApproval([ActivityTrigger]ApprovalRequest approvalRequest, ILogger logger)
        {
            return Task.CompletedTask;
        }
    }
}
