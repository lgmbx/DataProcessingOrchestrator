using Newtonsoft.Json;

namespace DataProcessingOrchestrator
{
    public class InputOrderModel
    {
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }
    }
}
