using Newtonsoft.Json;

namespace XRPL.DataRippleService.Exchanges
{
    public class ExchangeHistory
    {
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("count")]
        public uint Count { get; set; }
        [JsonProperty("marker")]
        public string Market { get; set; }
        public List<ExchangeObject> Exchanges { get; set; }
    }
}
