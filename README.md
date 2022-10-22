# XRPL.DataRippleService
XRPL data.ripple.com api

```Install-Package XRPL.DataRippleService -Version 1.0.0.0```

### Client with api to https://data.ripple.com

used https://github.com/ripple/rippled-historical-database

Ledger Contents Methods:

* [Get Ledger - `GET /v2/ledgers/{ledger_identifier}`](#get-ledger)
* [Get Transaction - `GET /v2/transactions/{hash}`](#get-transaction)
* [Get Transactions - `GET /v2/transactions/`](#get-transactions)
* [Get Payments - `GET /v2/payments/{currency}`](#get-payments)
- [x] * [Get Exchanges - `GET /v2/exchanges/{base}/{counter}`](#get-exchanges)
- [x] * [Get Exchange Rates - `GET /v2/exchange_rates/{base}/{counter}`](#get-exchange-rates)
* [Normalize - `GET /v2/normalize`](#normalize)
* [Get Daily Reports - `GET /v2/reports/`](#get-daily-reports)
* [Get Stats - `GET /v2/stats/`](#get-stats)
* [Get Active Accounts - `GET /v2/active_accounts/{base}/{counter}`](#get-active-accounts)
* [Get Exchange Volume - `GET /v2/network/exchange_volume`](#get-exchange-volume)
* [Get Payment Volume - `GET /v2/network/payment_volume`](#get-payment-volume)
* [Get External Markets - `GET /v2/network/external_markets`](#get-external-markets)
* [Get XRP Distribution - `GET /v2/network/xrp_distribution`](#get-xrp-distribution)
* [Get Top Currencies - `GET /v2/network/top_currencies`](#get-top-currencies)
* [Get Top Markets - `GET /v2/network/top_markets`](#get-top-markets)

Account Methods:

* [Get Account - `GET /v2/accounts/{address}`](#get-account)
* [Get Accounts - `GET /v2/accounts`](#get-accounts)
* [Get Account Balances - `GET /v2/accounts/{address}/balances`](#get-account-balances)
* [Get Account Orders - `GET /v2/accounts/{address}/orders`](#get-account-orders)
* [Get Account Transaction History - `GET /v2/accounts/{address}/transactions`](#get-account-transaction-history)
* [Get Transaction By Account and Sequence - `GET /v2/accounts/{address}/transactions/{sequence}`](#get-transaction-by-account-and-sequence)
* [Get Account Payments - `GET /v2/accounts/{address}/payments`](#get-account-payments)
- [x] * [Get Account Exchanges - `GET /v2/accounts/{address}/exchanges`](#get-account-exchanges)
- [x] * [Get Account Balance Changes - `GET /v2/accounts/{address}/balance_changes`](#get-account-balance-changes)
* [Get Account Reports - `GET /v2/accounts/{address}/reports`](#get-account-reports)
* [Get Account Transaction Stats - `GET /v2/accounts/{address}/stats/transactions`](#get-account-transaction-stats)
* [Get Account Value Stats - `GET /v2/accounts/{address}/stats/value`](#get-account-value-stats)

```C#
static async Task Test_AccountHistory()
{

    var ripple = new DataRippleClient();
    
    var history = await ripple.AccountExchanges(new AccounExchangesRequest()
    {
        Address = "rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p",
        BaseCurrency = new RippleServiceCurrency()
        {
            CurrencyCode = "xBay",
            Issuer = "rDVvK7xd2M6ZJr9a8suURWLqAeg7FyoDKT"
        },
        CounterCurrency = new RippleServiceCurrency() { CurrencyCode = "XRP" },
        Limit = 50,
        Descending = false,
        StartTime = new DateTime(2022, 05, 01),
        EndTime = DateTime.UtcNow
    });
    Console.WriteLine(JObject.Parse(JsonConvert.SerializeObject(history)));

}

static async Task Test_AccountBalanceChanges()
{

    var ripple = new DataRippleClient();

    var values = await ripple.AccountBalanceChangesJson(
        new AccountBalanceChangesRequest()
        {
            Address = "rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p",
            Currency = "MRM",
            StartTime = null,
            EndTime = null,
            Format = DataRippleResponseFormat.csv,
            counterparty = "rNjQ9HZYiBk1WhuscDkmJRSc3gbrBqqAaQ",
            Descending = false,
            Limit = 100
        });

    var map = values.GroupByMaximumInDay().ToArray();
    Console.WriteLine(JsonConvert.SerializeObject(map));

}
```
