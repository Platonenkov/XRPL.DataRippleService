using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRPL.DataRippleService.Exchanges
{
    public class ExchangesRequest: DataRippleBaseRequest
    {
        public RippleServiceCurrency BaseCurrency { get; set; }
        public RippleServiceCurrency CounterCurrency { get; set; }
        /// <summary>
        /// Aggregation interval: 1minute, 5minute, 15minute, 30minute, 1hour, 2hour, 4hour, 1day, 3day, 7day, or 1month. <br/>
        /// The default is non-aggregated results.
        /// </summary>
        public string Interval { get; set; }
        /// <summary>
        /// If true, aggregate all individual results. The default is false.
        /// </summary>
        public bool Reduce { get; set; }
        /// <summary>
        /// If true, filter results to autobridged exchanges only.
        /// </summary>
        public bool AutoBridged { get; set; }
    }
}
