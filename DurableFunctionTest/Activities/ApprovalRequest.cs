namespace DurableFunctionTest.Activities
{
    public class ApprovalRequest
    {
        public string DurableContextId { get; set; }
        public string LicencePlate { get; set; }
        public float AccuracyRecognition { get; set; }
        public string LicenPlateUrl { get; set; }
    }
}