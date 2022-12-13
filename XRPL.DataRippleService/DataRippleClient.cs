using System.Net;
using System.Threading.Channels;

using XRPL.DataRippleService.Balances;
using XRPL.DataRippleService.Exchanges;
using XRPL.DataRippleService.Request;

namespace XRPL.DataRippleService
{
    public class DataRippleClient : BaseClient
    {

        public DataRippleClient() : base("http://data.ripple.com/")
        {
        }
        /// <summary>
        /// Retrieve an exchange rate for a given currency pair at a specific time.
        /// </summary>
        /// <param name="buy">Base currency of the pair, as a Currency Code, followed by + and the issuer Address. Omit the + and the issuer for XRP.</param>
        /// <param name="pay">Counter currency of the pair, as a Currency Code, followed by + and the issuer Address. Omit the + and the issuer for XRP.</param>
        /// <returns></returns>
        public async Task<double> ExchangesRates(RippleServiceCurrency buy, RippleServiceCurrency pay)
        {
            var buy_name = buy.CurrencyCode == "XRP" ? "XRP" : $"{buy.CurrencyCode}+{buy.Issuer}";
            var pay_name = pay.CurrencyCode == "XRP" ? "XRP" : $"{pay.CurrencyCode}+{pay.Issuer}";
            string server = $"v2/exchange_rates/{buy_name}/{pay_name}";
            var result = await GetAsync<ExchangeRate>(server);
            return result.Data.Rate;
        }

        /// <summary>
        /// Retrieve Exchanges for a given currency pair over time. Results can be returned as individual exchanges or aggregated to a specific list of intervals
        /// </summary>
        /// <param name="buy">Base currency of the pair, as a Currency Code, followed by + and the issuer Address unless it's XRP.</param>
        /// <param name="pay">Counter currency of the pair, as a Currency Code, followed by + and the issuer Address unless it's XRP.</param>
        /// <param name="limit">limit</param>
        /// <param name="descending">sorting</param>
        /// <returns></returns>
        public async Task<ServerResponse<DataRippleExchangesResponse>> Exchanges(RippleServiceCurrency buy, RippleServiceCurrency pay, int limit = 100, bool descending = true)
        {
            var buy_name = buy.CurrencyCode == "XRP" ? "XRP" : $"{buy.CurrencyCode}+{buy.Issuer}";
            var pay_name = pay.CurrencyCode == "XRP" ? "XRP" : $"{pay.CurrencyCode}+{pay.Issuer}";
            string server = $"v2/exchanges/{buy_name}/{pay_name}?descending={descending}&limit={limit}";
            var result = await GetAsync<DataRippleExchangesResponse>(server);
            return result;
        }
        /// <summary>
        /// Retrieve Exchanges for a given currency pair over time. Results can be returned as individual exchanges or aggregated to a specific list of intervals
        /// </summary>
        /// <param name="request">request form</param>
        /// <param name="Progress"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        public async Task<ServerResponse<DataRippleExchangesResponse>> Exchanges(ExchangesRequest request,
            IProgress<(double?, string, string, bool?)> Progress = null, CancellationToken Cancel = default)
        {
            var result = await ExchangesBase(request, Progress, Cancel);
            Cancel.ThrowIfCancellationRequested();
            if (!result.HasData)
                return result;
            Progress?.Report((null, $"Got {result.Data.count},{Environment.NewLine}marker: {result.Data.marker}", null, null)!);
            if (string.IsNullOrWhiteSpace(result.Data.marker))
                return result;
            var marker = result.Data.marker;
            var counter = 0;
            while (!string.IsNullOrWhiteSpace(marker))
            {
                request.Marker = marker;
                Cancel.ThrowIfCancellationRequested();
                var add = await ExchangesBase(request, Progress, Cancel);
                switch (add.HasData)
                {
                    case true: break;
                    //todo null when to many request
                    case false when counter > 5: return result;
                    case false when counter <= 5:
                        {
                            await Task.Delay(2000, Cancel);
                            counter++;
                            continue;
                        }
                }

                Progress?.Report((null, $"Got {add.Data.count},{Environment.NewLine}marker: {add.Data.marker}", null, null)!);

                counter = 0;
                marker = add.Data.marker;
                result.Data.exchanges.AddRange(add.Data.exchanges);
                result.Data.count += add.Data.count;
            }
            return result;

        }
        /// <summary>
        /// Retrieve Exchanges for a given currency pair over time. Results can be returned as individual exchanges or aggregated to a specific list of intervals
        /// </summary>
        /// <param name="request">request form<</param>
        /// <param name="Progress"></param>
        /// <param name="Cancel"></param>
        /// <returns></returns>
        private async Task<ServerResponse<DataRippleExchangesResponse>> ExchangesBase(ExchangesRequest request,
            IProgress<(double?, string, string, bool?)> Progress = null, CancellationToken Cancel = default)
        {
            Cancel.ThrowIfCancellationRequested();
            var buy_name = request.BaseCurrency.CurrencyCode == "XRP" ? "XRP" : $"{request.BaseCurrency.CurrencyCode}+{request.BaseCurrency.Issuer}";
            var pay_name = request.CounterCurrency.CurrencyCode == "XRP" ? "XRP" : $"{request.CounterCurrency.CurrencyCode}+{request.CounterCurrency.Issuer}";
            string server = $"v2/exchanges/{buy_name}/{pay_name}?format={request.Format}";
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
            if (request.AutoBridged is { } auto)
                server += $"&autobridged={auto}";
            if (request.Reduce is { } reduce)
                server += $"&reduce={reduce}";
            if (request.Interval is { } interval)
                server += $"&interval={interval}";
            var result = await GetAsync<DataRippleExchangesResponse>(server, Cancel);
            return result;
        }

        /// <summary>
        /// Retrieve Balance changes for a given account over time.
        /// </summary>
        /// <param name="request">request form</param>
        /// <param name="SkipLimit">if true, will receive full data without limiting data </param>
        /// <returns></returns>
        public async Task<ServerResponse<BalanceChangeResponse>> AccountBalanceChangesJson(AccountBalanceChangesRequest request, bool SkipLimit = false,
            IProgress<(double?, string, string, bool?)> Progress = null, CancellationToken Cancel = default)
        {
            var response = await AccountBalanceChangesJsonBase(request, Cancel);
            if (response is null)
                return null;
            if (!response.HasData)
                return response;
            Cancel.ThrowIfCancellationRequested();
            Progress?.Report((null, $"Got {response.Data.count},{Environment.NewLine}marker: {response.Data.marker}", null, null));
            if (!SkipLimit)
                return response;

            var marker = response.Data.marker;
            var counter = 0;
            while (!string.IsNullOrWhiteSpace(marker))
            {
                Cancel.ThrowIfCancellationRequested();
                request.Marker = marker;
                var new_response = await AccountBalanceChangesJsonBase(request, Cancel);
                Cancel.ThrowIfCancellationRequested();

                switch (new_response.HasData)
                {
                    case true: break;
                    //todo null when to many request
                    case false when counter > 5: return response;
                    case false when counter <= 5:
                        await Task.Delay(2000, Cancel);
                        counter++;
                        continue;
                }

                Progress?.Report((null, $"Got {new_response.Data.count},{Environment.NewLine}marker: {new_response.Data.marker}", null, null)!);
                counter = 0;

                marker = new_response.Data.marker;
                response.Data.balance_changes.AddRange(new_response.Data.balance_changes);
                response.Data.count = response.Data.balance_changes.Count;
            }

            return response;
        }
        /// <summary>
        /// Retrieve Balance changes for a given account over time.<br/>
        /// GET /v2/accounts/{address}/balance_changes/
        /// </summary>
        /// <param name="request">request form</param>
        /// <returns></returns>
        public async Task<ServerResponse<BalanceChangeResponse>> AccountBalanceChangesJsonBase(AccountBalanceChangesRequest request, CancellationToken Cancel)
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

            var response = await GetAsync<BalanceChangeResponse>(server, Cancel);

            return response;
        }
        /// <summary>
        /// Retrieve Exchanges for a given account over time.<br/>
        /// https://github.com/ripple/rippled-historical-database#get-exchanges
        /// </summary>
        /// <param name="request">request, ATTENTION - possible null when descending == true</param>
        /// <returns></returns>
        public async Task<ServerResponse<DataRippleExchangesResponse>> AccountExchanges(AccounExchangesRequest request,
            IProgress<(double?, string, string, bool?)> Progress = null, CancellationToken Cancel = default)
        {
            Cancel.ThrowIfCancellationRequested();
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


            var result = await GetAsync<DataRippleExchangesResponse>(server, Cancel);
            Cancel.ThrowIfCancellationRequested();
            if (!result.HasData)
                return result;
            Progress?.Report((null, $"Got {result.Data.count},{Environment.NewLine}marker: {result.Data.marker}", null, null)!);
            if (string.IsNullOrWhiteSpace(result.Data.marker))
                return result;
            var marker = result.Data.marker;
            var counter = 0;
            while (!string.IsNullOrWhiteSpace(marker))
            {
                Cancel.ThrowIfCancellationRequested();
                var add = await GetAsync<DataRippleExchangesResponse>($"{server}&marker={marker}", Cancel);
                switch (add.HasData)
                {
                    case true: break;
                    //todo null when to many request
                    case false when counter > 5: return result;
                    case false when counter <= 5:
                        await Task.Delay(2000, Cancel);

                        counter++;
                        continue;
                }

                Progress?.Report((null, $"Got {add.Data.count},{Environment.NewLine}marker: {add.Data.marker}", null, null)!);

                counter = 0;
                marker = add.Data.marker;
                result.Data.exchanges.AddRange(add.Data.exchanges);
                result.Data.count += add.Data.count;
            }
            return result;
        }


        public async Task<ServerResponse<DataRippleAccountBalancesResponse>> AccountBalances(
            BalancesRequest request,
            IProgress<(double?, string, string, bool?)> Progress = null, CancellationToken Cancel = default)
        {
            Cancel.ThrowIfCancellationRequested();
            string server = $"/v2/accounts/{request.Address}/balances?format={request.Format}";
            if (!string.IsNullOrWhiteSpace(request.ledger_hash))
                server += $"&ledger_hash={request.ledger_hash}";
            if (request.ledger_index is { })
                server += $"&ledger_index={request.ledger_index}";
            if (request.Date is { })
                server += $"&date={request.date}";
            if (!string.IsNullOrWhiteSpace(request.Currency))
                server += $"&currency={request.Currency}";
            if (!string.IsNullOrWhiteSpace(request.Counterparty))
                server += $"&counterparty={request.Counterparty}";
            if (!string.IsNullOrWhiteSpace(request.Marker))
                server += $"&marker={request.Marker}";
            if (request.Limit is { } limit)
                server += $"&limit={limit}";
            else
                server += $"&limit=all";


            var result = await GetAsync<DataRippleAccountBalancesResponse>(server, Cancel);
            Cancel.ThrowIfCancellationRequested();
            if (!result.HasData)
                return result;
            Progress?.Report((null, $"Got {result.Data.balances.Count},{Environment.NewLine}marker: {result.Data.marker}", null, null)!);
            if (string.IsNullOrWhiteSpace(result.Data.marker))
                return result;
            var marker = result.Data.marker;
            var counter = 0;
            while (!string.IsNullOrWhiteSpace(marker))
            {
                Cancel.ThrowIfCancellationRequested();
                var add = await GetAsync<DataRippleAccountBalancesResponse>($"{server}&marker={marker}", Cancel);
                switch (add.HasData)
                {
                    case true: break;
                    //todo null when to many request
                    case false when counter > 5: return result;
                    case false when counter <= 5:
                        await Task.Delay(2000, Cancel);

                        counter++;
                        continue;
                }

                Progress?.Report((null, $"Got {add.Data.balances.Count},{Environment.NewLine}marker: {add.Data.marker}", null, null)!);

                counter = 0;
                marker = add.Data.marker;
                result.Data.balances.AddRange(add.Data.balances);
            }
            return result;

        }
    }
}
