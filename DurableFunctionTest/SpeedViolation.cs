using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionTest
{
    public class SpeedViolation
    {
        public int CamaraId { get; set; }
        public string LicensePlate { get; set; }

        public float AccuracyRecognition { get; set; }

        public int Speed { get; set; }
    }
}
