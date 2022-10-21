namespace XRPL.DataRippleService.Request;

/// <summary>
/// https://github.com/ripple/rippled-historical-database#get-account-exchanges
/// Retrieve Exchanges for a given account over time.
/// </summary>
public class AccounExchangesRequest : DataRippleBaseRequest
{
    /// <summary>
    /// XRP Ledger address to query.
    /// </summary>
    public string Address { get; set; }
    /// <summary>
    /// Base Currency Code
    /// </summary>
    public RippleServiceCurrency BaseCurrency { get; set; }
    /// <summary>
    /// CounterCurrency Code
    /// </summary>
    public RippleServiceCurrency CounterCurrency { get; set; }

}