using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionTest.Contracts
{
    public class ManualUserRequest
    {
        public string DurableInstanceId { get; set; }
        public bool Approved { get; set; }
        public string LicesePlate { get; set; }
    }
}
