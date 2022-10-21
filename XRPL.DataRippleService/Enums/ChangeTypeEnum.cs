namespace XRPL.DataRippleService.Enums;

/// <summary>
/// The following values are valid for the change_type field of a Balance Change Descriptor
/// </summary>
public enum ChangeTypeEnum
{
    None,
    /// <summary>
    /// This balance change reflects XRP that was destroyed to relay a transaction. (Prior to v2.0.4, this was network fee instead.)
    /// </summary>
    transaction_cost,
    /// <summary>
    /// This balance change reflects currency that was received from a payment.
    /// </summary>
    payment_destination,
    /// <summary>
    /// This balance change reflects currency that was spent in a payment.
    /// </summary>
    payment_source,
    /// <summary>
    /// This balance change reflects currency that was traded for other currency,
    /// or the same currency from a different issuer. This can occur in the middle of payment execution as well as from offers.
    /// </summary>
    exchange,
    /// <summary>
    /// The intermediary in the transaction, is usually billed to the token issuer
    /// </summary>
    intermediary
}