using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XRPL.DataRippleService.Enums;

namespace XRPL.DataRippleService.Balances
{
    public class BalancesRequest
    {
        /// <summary>
        /// UTC date for historical balances.
        /// Standardized UTC format yyyy-MM-ddTHH:mm:ssZ
        /// </summary>
        public string date => Date is { } time ? $"{time:yyyy-MM-ddTHH:mm:ssZ}" : null;
        /// <summary>
        /// UTC date for historical balances.
        /// </summary>
        public DateTime? Date { get; set; }


        /// <summary>
        /// Maximum results per page.<br/>
        /// The default is 200.<br/>
        /// Cannot be greater than 400, but you can use the value all to return all results.<br/>
        /// (Caution: When using limit=all to retrieve very many results, the request may time out.<br/>
        /// For large issuers, there can be several tens of thousands of results.)
        /// </summary>
        public uint? Limit { get; set; }
        /// <summary>
        /// Pagination key from previously returned response.
        /// </summary>
        public string Marker { get; set; }

        /// <summary>
        /// Format of returned results: csv or json. The default is json.
        /// </summary>
        public DataRippleResponseFormat? Format { get; set; } = DataRippleResponseFormat.json;

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
        public string Counterparty { get; set; }
        /// <summary>
        /// Index of ledger for historical balances.
        /// </summary>
        public int? ledger_index { get; set; }
        /// <summary>
        /// Ledger hash for historical balances.
        /// </summary>
        public string ledger_hash { get; set; }
    }
}
