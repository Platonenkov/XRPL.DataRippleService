using Newtonsoft.Json;
using XRPL.DataRippleService.Enums;

namespace XRPL.DataRippleService.Exchanges;

public class BalanceChangeResponse : DataRippleBaseResponse
{
    public List<BalanceChangeData> balance_changes { get; set; }
}

public class BalanceSnapshot
{
    public DateTime Date { get; set; }
    public RippleServiceCurrency Currency { get; set; }
}

public class BalanceChangeData
{
    /// <summary> Transaction change type  </summary>
    public BalanceChangeResultEnum TransactionChangeStatus => ChangeType switch
    {
        ChangeTypeEnum.None
        or ChangeTypeEnum.transaction_cost
        or ChangeTypeEnum.intermediary => BalanceChangeResultEnum.none,
        ChangeTypeEnum.payment_destination => BalanceChangeResultEnum.Receive,
        ChangeTypeEnum.payment_source => BalanceChangeResultEnum.Sent,
        ChangeTypeEnum.exchange when amount_change >= 0 => BalanceChangeResultEnum.Bought,
        ChangeTypeEnum.exchange when amount_change < 0 => BalanceChangeResultEnum.Sold,
        _ => throw new ArgumentOutOfRangeException()
    };

    /// <summary>
    /// The difference in the amount of currency held before and after this change.
    /// </summary>
    public double amount_change { get; set; }
    /// <summary>
    /// The balance after the change occurred.
    /// </summary>
    public decimal final_balance { get; set; }
    /// <summary>
    /// This balance change is represented by the entry at this index of the ModifiedNodes array within the metadata section of the transaction that executed this balance change.<br/>
    /// Note: When the transaction cost is combined with other changes to XRP balance, the transaction cost has a node_index of null instead.
    /// </summary>
    public string node_index { get; set; }
    /// <summary>
    /// The transaction that executed this balance change is at this index in the array of transactions for the ledger that included it.
    /// </summary>
    public string tx_index { get; set; }
    /// <summary>
    /// One of several describing what caused this balance change to occur.
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    [JsonProperty("change_type")]
    public ChangeTypeEnum ChangeType { get; set; }
    /// <summary>
    /// The change affected this currency.
    /// </summary>
    public string currency { get; set; }
    /// <summary>
    /// The time the change occurred.
    /// (This is based on the close time of the ledger that included the transaction that executed the change.
    /// </summary>
    public DateTime? executed_time { get; set; }
    /// <summary>
    /// (Omitted for XRP) The currency is held in a trust line to or from this account. (Prior to v2.0.6, this field was called issuer.)
    /// </summary>
    public string counterparty { get; set; }
    /// <summary>
    /// The sequence number of the ledger that included the transaction that executed this balance change.
    /// </summary>
    public string ledger_index { get; set; }
    /// <summary>
    /// The identifying hash of the transaction that executed this balance change.
    /// </summary>
    public string tx_hash { get; set; }
}

