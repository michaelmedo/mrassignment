using Newtonsoft.Json;

namespace MediaRadar.API.SDK.Models.PubAdActivities
{
    public class PubAdActivity
    {
        [JsonProperty("Month")]
        public string Month { get; set; }

        [JsonProperty("PublicationId")]
        public int PublicationId { get; set; }

        [JsonProperty("PublicationName")]
        public string PublicationName { get; set; }

        [JsonProperty("ParentCompany")]
        public string ParentCompany { get; set; }

        [JsonProperty("ParentCompanyId")]
        public int ParentCompanyId { get; set; }

        [JsonProperty("BrandName")]
        public string BrandName { get; set; }

        [JsonProperty("BrandId")]
        public int BrandId { get; set; }

        [JsonProperty("ProductCategory")]
        public string ProductCategory { get; set; }

        [JsonProperty("AdPages")]
        public double AdPages { get; set; }

        [JsonProperty("EstPrintSpend")]
        public int EstPrintSpend { get; set; }
    }
}
