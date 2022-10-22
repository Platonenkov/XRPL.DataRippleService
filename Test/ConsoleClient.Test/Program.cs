using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using XRPL.DataRippleService;
using XRPL.DataRippleService.Enums;
using XRPL.DataRippleService.Exchanges;
using XRPL.DataRippleService.Extensions;
using XRPL.DataRippleService.Request;




await Test_BookExchanges();

await Test_AccountHistory();
await Test_AccountBalanceChanges();

Console.WriteLine("Hello, from XrplDaddy!");


static async Task Test_BookExchanges()
{

    var progress = new Progress<(double? percent, string message, string title, bool? CanCancel)>(
        (p) =>
        {
            Console.WriteLine(p.Item2);
        });
    var ripple = new DataRippleClient();
    var request = new ExchangesRequest()
    {
        BaseCurrency = new RippleServiceCurrency()
        {
            CurrencyCode = "AnimaCoin",
            Issuer = "rGQrZvndQsJV2S5cnSdiRFMPT1Fz1Ccvuj"
        },
        CounterCurrency = new RippleServiceCurrency(),
        StartTime = DateTime.Now.Date,
        EndTime = DateTime.Now
    };

    var history = await ripple.Exchanges(request, progress);
    //var history = await ripple.Exchanges(new ExchangesRequest()
    //{
    //    BaseCurrency = new RippleServiceCurrency()
    //    {
    //        Issuer = "rGQrZvndQsJV2S5cnSdiRFMPT1Fz1Ccvuj",
    //        CurrencyCode = "AnimaCoin"
    //    },
    //    CounterCurrency = new RippleServiceCurrency()
    //    {
    //        CurrencyCode = "XRP"
    //    },
    //    StartTime = DateTime.Now.ToUniversalTime() - TimeSpan.FromHours(1),
    //    EndTime = null,
    //}, progress);
    Console.WriteLine(JObject.Parse(JsonConvert.SerializeObject(history)));

}
static async Task Test_AccountHistory()
{
    var progress = new Progress<(double? percent, string message, string title, bool? CanCancel)>(
        (p) =>
        {
            Console.WriteLine(p.Item2);
        });

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
    }, progress);
    Console.WriteLine(JObject.Parse(JsonConvert.SerializeObject(history)));

}

static async Task Test_AccountBalanceChanges()
{

    var ripple = new DataRippleClient();
    var progress = new Progress<(double? percent, string message, string title, bool? CanCancel)>(
        (p) =>
        {
            Console.WriteLine(p.Item2);
        });
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
        }, Progress: progress);

    var map = values.GroupByMaximumInDay().ToArray();
    Console.WriteLine(JsonConvert.SerializeObject(map));

}
