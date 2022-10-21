namespace XRPL.DataRippleService.Request
{
    /// <summary>
    /// https://github.com/ripple/rippled-historical-database#get-account-balance-changes
    /// Request for Retrieve Balance changes for a given account over time
    /// </summary>
    public class AccountBalanceChangesRequest: DataRippleBaseRequest
    {
        /// <summary>
        /// XRP Ledger address to query.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Restrict results to specified currency.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Restrict results to specified counterparty/issuer.
        /// </summary>
        public string counterparty { get; set; }

    }
}
