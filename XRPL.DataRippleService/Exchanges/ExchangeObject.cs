using Newtonsoft.Json;

namespace XRPL.DataRippleService.Exchanges;

public class ExchangeObject
{
    /// <summary>
    /// The amount of the base currency that was traded.
    /// </summary>
    public double base_amount { get; set; }
    /// <summary>
    /// The amount of the counter currency that was traded.
    /// </summary>
    public double counter_amount { get; set; }
    /// <summary>
    /// The amount of the counter currency acquired per 1 unit of the base currency.
    /// </summary>
    public double rate { get; set; }
    /// <summary>
    /// May be omitted) If the offer was autobridged (XRP order books were used to bridge two non-XRP currencies),
    /// this is the other currency from the offer that executed this exchange..<br/>
    /// Currency Code
    /// </summary>
    public string autobridged_currency { get; set; }
    /// <summary>
    /// (May be omitted) If the offer was autobridged (XRP order books were used to bridge two non-XRP currencies), this is the other currency from the offer that executed this exchange.
    /// </summary>
    public string autobridged_issuer { get; set; }
    /// <summary>
    /// The base currency.<br/>
    /// Currency Code
    /// </summary>
    public string base_currency { get; set; }
    /// <summary>
    /// (Omitted for XRP) The account that issued the base currency.
    /// </summary>
    public string base_issuer { get; set; }
    /// <summary>
    /// The account that acquired the base currency.
    /// </summary>
    public string buyer { get; set; }
    /// <summary>
    /// (May be omitted) If the transaction contains a memo indicating what client application sent it, this is the contents of the memo
    /// </summary>
    public string client { get; set; }
    /// <summary>
    /// The counter currency.<br/>
    /// Currency Code
    /// </summary>
    public string counter_currency { get; set; }
    /// <summary>
    /// (Omitted for XRP) The account that issued the counter currency.
    /// </summary>
    public string counter_issuer { get; set; }
    /// <summary>
    /// The time the exchange occurred.<br/>
    /// All dates and times are written in ISO 8601 Timestamp Format, using UTC.<br/>
    /// This format is summarized as:<br/>
    ///YYYY-MM-DDThh:mm:ssZ<br/>
    /// Four-digit year<br/>
    /// Two-digit month<br/>
    /// Two-digit day<br/>
    /// The letter T separating the date part and the time part<br/>
    /// Two-digit hour using a 24-hour clock<br/>
    /// Two digit minute<br/>
    /// The letter Z indicating zero offset from UTC.<br/>
    /// </summary>
    public DateTime? executed_time { get; set; }
    /// <summary>
    /// The sequence number of the ledger that included this transaction.
    /// </summary>
    public string ledger_index { get; set; }
    /// <summary>
    /// The sequence number of the provider's existing offer in the ledger.
    /// </summary>
    public ulong offer_sequence { get; set; }
    /// <summary>
    /// The account that had an existing Offer in the ledger.
    /// </summary>
    public string provider { get; set; }
    /// <summary>
    /// The account that acquired the counter currency.
    /// </summary>
    public string seller { get; set; }
    /// <summary>
    /// The account that sent the transaction which executed this exchange.
    /// </summary>
    public string taker { get; set; }
    /// <summary>
    /// The identifying hash of the transaction that executed this exchange.<br/>
    /// (Note: This exchange may be one of several executed in a single transaction.)
    /// </summary>
    public string tx_hash { get; set; }
    /// <summary>
    /// The type of transaction that executed this exchange, either Payment or OfferCreate.
    /// </summary>
    public string tx_type { get; set; }


    /// <summary>
    /// 
    /// </summary>
    public string node_index { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string tx_index { get; set; }
    [JsonIgnore]
    public bool IsSellOrder => seller == taker;
}