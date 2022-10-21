using System.Globalization;
using Newtonsoft.Json;

namespace XRPL.DataRippleService.Exchanges;

public class ExchangeRate
{
    public string result { get; set; }
    public string rate { get; set; }
    [JsonIgnore]
    public double Rate => double.Parse(rate, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
}