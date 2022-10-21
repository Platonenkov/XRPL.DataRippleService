using XRPL.DataRippleService.Exchanges;
using XRPL.DataRippleService.Request;

namespace XRPL.DataRippleService
{
    public class DataRippleClient : BaseClient
    {

        public DataRippleClient() : base("http://data.ripple.com/")
        {
        }

        public async Task<double> ExchangesRates(RippleServiceCurrency buy, RippleServiceCurrency pay)
        {
            var buy_name = buy.CurrencyCode == "XRP" ? "XRP" : $"{buy.CurrencyCode}+{buy.Issuer}";
            var pay_name = pay.CurrencyCode == "XRP" ? "XRP" : $"{pay.CurrencyCode}+{pay.Issuer}";
            string server = $"v2/exchange_rates/{buy_name}/{pay_name}";
            var result = await GetAsync<ExchangeRate>(server);
            return result.Rate;
        }
        public async Task<DataRippleExchangesResponse> Exchanges(RippleServiceCurrency buy, RippleServiceCurrency pay, int limit = 50, bool descending = true)
        {
            var buy_name = buy.CurrencyCode == "XRP" ? "XRP" : $"{buy.CurrencyCode}+{buy.Issuer}";
            var pay_name = pay.CurrencyCode == "XRP" ? "XRP" : $"{pay.CurrencyCode}+{pay.Issuer}";
            string server = $"v2/exchanges/{buy_name}/{pay_name}?descending={descending}&limit={limit}";
            var result = await GetAsync<DataRippleExchangesResponse>(server);
            return result;
        }

        /// <summary>
        /// Retrieve Balance changes for a given account over time.
        /// </summary>
        /// <param name="request">request form</param>
        /// <param name="SkipLimit">if true, will receive full data without limiting data </param>
        /// <returns></returns>
        public async Task<List<BalanceChangeData>> AccountBalanceChangesJson(AccountBalanceChangesRequest request, bool SkipLimit = false)
        {
            var response = await AccountBalanceChangesJsonBase(request);
            if (response is null)
                return null;
            if (!SkipLimit)
                return response.balance_changes;

            var result = response.balance_changes;
            var marker = response.marker;
            var counter = 0;
            while (!string.IsNullOrWhiteSpace(marker))
            {
                request.Marker = marker;
                response = await AccountBalanceChangesJsonBase(request);
                if (response is null && counter > 5) //todo null when to many request
                    return result;
                if (response is null && counter <= 5)
                {
                    counter++;
                    continue;
                }
                counter = 0;

                marker = response.marker;
                result.AddRange(response.balance_changes);
            }

            return result;
        }
        /// <summary>
        /// Retrieve Balance changes for a given account over time.<br/>
        /// GET /v2/accounts/{address}/balance_changes/
        /// </summary>
        /// <param name="request">request form</param>
        /// <returns></returns>
        public async Task<BalanceChangeResponse> AccountBalanceChangesJsonBase(AccountBalanceChangesRequest request)
        {
            string server = $"v2/accounts/{request.Address}/balance_changes?";
            if (!string.IsNullOrWhiteSpace(request.Currency))
                server += $"&currency={request.Currency}";
            if (!string.IsNullOrWhiteSpace(request.counterparty))
                server += $"&counterparty={request.counterparty}";
            if (request.StartTime is { })
                server += $"&start={request.Start}";
            if (request.EndTime is { })
                server += $"&end={request.End}";
            if (!string.IsNullOrWhiteSpace(request.Marker))
                server += $"&marker={request.Marker}";
            if (request.Limit is { } limit)
                server += $"&limit={limit}";
            if (request.Descending is { } des)
                server += $"&descending={des}";

            //if (request.Format is { } format)
            server += "&format=json";

            var response = await GetAsync<BalanceChangeResponse>(server);

            return response;
        }
        /// <summary>
        /// Retrieve Exchanges for a given account over time.<br/>
        /// https://github.com/ripple/rippled-historical-database#get-exchanges
        /// </summary>
        /// <param name="request">request, ATTENTION - possible null when descending == true</param>
        /// <returns></returns>
        public async Task<DataRippleExchangesResponse> AccountExchanges(AccounExchangesRequest request)
        {
            var buy_name = request.BaseCurrency.CurrencyCode == "XRP" ? "XRP" : $"{request.BaseCurrency.CurrencyCode}+{request.BaseCurrency.Issuer}";
            var pay_name = request.CounterCurrency.CurrencyCode == "XRP" ? "XRP" : $"{request.CounterCurrency.CurrencyCode}+{request.CounterCurrency.Issuer}";
            string server = $"v2/accounts/{request.Address}/exchanges/{buy_name}/{pay_name}?format={request.Format}";
            if (request.StartTime is { })
                server += $"&start={request.Start}";
            if (request.EndTime is { })
                server += $"&end={request.End}";
            if (!string.IsNullOrWhiteSpace(request.Marker))
                server += $"&marker={request.Marker}";
            if (request.Limit is { } limit)
                server += $"&limit={limit}";
            if (request.Descending is { } des)
                server += $"&descending={des}";


            var result = await GetAsync<DataRippleExchangesResponse>(server);
            if (result is null)
                return null;
            if (string.IsNullOrWhiteSpace(result.marker))
                return result;
            var marker = result.marker;
            var counter = 0;
            while (!string.IsNullOrWhiteSpace(marker))
            {
                var add = await GetAsync<DataRippleExchangesResponse>($"{server}&marker={marker}");
                if (add is null && counter > 5) //todo null when to many request
                    return result;
                if (add is null && counter <= 5)
                {
                    counter++;
                    continue;
                }

                counter = 0;
                marker = add.marker;
                result.exchanges.AddRange(add.exchanges);
                result.count += add.count;
            }
            return result;
        }

    }
}
