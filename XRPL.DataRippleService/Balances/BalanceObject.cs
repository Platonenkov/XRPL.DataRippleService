using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using XRPL.DataRippleService.Extensions;

namespace XRPL.DataRippleService.Exchanges;

public class BalanceObject
{
    public BalanceObject()
    {
        CurrencyCode = "XRP";
    }

    private string _CurrencyCode;

    [JsonProperty("currency")]
    public string CurrencyCode
    {
        get => _CurrencyCode;
        set
        {
            var cur_code = value.Trim();
            if (cur_code.Length <= 3)
            {
                _CurrencyCode = cur_code;
                return;
            }
            if (IsHexCurrencyCode(cur_code))
            {
                _CurrencyCode = cur_code;
                return;
            }
            cur_code = cur_code.ToHex();
            if (cur_code.Length > 40)
                throw new ArgumentException($"CurrencyCode can be 40 character maximum\nCode: {cur_code}", nameof(CurrencyCode));

            cur_code += new string('0', 40 - cur_code.Length);



            _CurrencyCode = cur_code;
        }
    }

    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("counterparty")]
    public string Issuer { get; set; }

    [JsonIgnore]
    public decimal ValueAsNumber
    {
        get => string.IsNullOrEmpty(Value)
            ? 0
            : decimal.Parse(Value,
                NumberStyles.AllowLeadingSign
                | (NumberStyles.AllowLeadingSign & NumberStyles.AllowDecimalPoint)
                | (NumberStyles.AllowLeadingSign & NumberStyles.AllowExponent)
                | (NumberStyles.AllowLeadingSign & NumberStyles.AllowExponent & NumberStyles.AllowDecimalPoint)
                | (NumberStyles.AllowExponent & NumberStyles.AllowDecimalPoint)
                | NumberStyles.AllowExponent
                | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        set => Value = value.ToString(CurrencyCode == "XRP" ? "G0" : "G15", CultureInfo.InvariantCulture);
    }

    [JsonIgnore]
    public decimal? ValueAsXrp
    {
        get
        {
            if (CurrencyCode != "XRP" || string.IsNullOrEmpty(Value))
                return null;
            return ValueAsNumber / 1000000;
        }
        set
        {
            if (value.HasValue)
            {
                CurrencyCode = "XRP";
                decimal val = value.Value * 1000000;
                Value = val.ToString("G0", CultureInfo.InvariantCulture);
            }
            else
            {
                Value = "0";
            }
        }
    }
    [JsonIgnore]
    public string CurrencyValidName => CurrencyCode is { Length: > 0 } row ? row.Length > 3 ? row.FromHexString().Trim('\0') : row : string.Empty;

    #region Overrides of Object

    public override string ToString() => CurrencyValidName == "XRP" ? $"XRP: {ValueAsXrp:0.######}" : $"{CurrencyValidName}: {ValueAsNumber:0.###############}";

    #endregion
    /// <summary>
    /// check currency code for HEX 
    /// </summary>
    /// <param name="code">currency code</param>
    /// <returns></returns>
    private static bool IsHexCurrencyCode(string code) => Regex.IsMatch(code, @"[0-9a-fA-F]{40}", RegexOptions.IgnoreCase);

}