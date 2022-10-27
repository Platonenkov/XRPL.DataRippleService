using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using XRPL.DataRippleService;
using XRPL.DataRippleService.Balances;
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
    var request = new BalancesRequest()
    {
        //Currency = "RLT",
        //Counterparty = "rUetS7kbVYJZ76za5ywa1DgViNZMgT9Bvq",
        Address = "rNcqT1tds4vdroichNKuTh3Ppd8KAdFHnN",
        Date = DateTime.Now - TimeSpan.FromDays(10), Limit = 400
    };
    
    var responce = await ripple.AccountBalances(request, progress);
    Console.WriteLine($"Server: {responce.Response.StatusCode}, message: {responce.Response.ReasonPhrase}");
    Console.WriteLine(responce.HasData
        ? JsonConvert.SerializeObject(responce.Data.balances)
        : "NO DATA");

}
static async Task Test_AccountHistory()
{
    var progress = new Progress<(double? percent, string message, string title, bool? CanCancel)>(
        (p) =>
        {
            Console.WriteLine(p.Item2);
        });

    var ripple = new DataRippleClient();

    var responce = await ripple.AccountExchanges(new AccounExchangesRequest()
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
    Console.WriteLine($"Server: {responce.Response.StatusCode}, message: {responce.Response.ReasonPhrase}");
    Console.WriteLine(responce.HasData 
        ? JsonConvert.SerializeObject(responce.Data.exchanges) 
        : "NO DATA");
}

static async Task Test_AccountBalanceChanges()
{

    var ripple = new DataRippleClient();
    var progress = new Progress<(double? percent, string message, string title, bool? CanCancel)>(
        (p) =>
        {
            Console.WriteLine(p.Item2);
        });
    var responce = await ripple.AccountBalanceChangesJson(
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
    Console.WriteLine($"Server: {responce.Response.StatusCode}, message: {responce.Response.ReasonPhrase}");
    if (responce.HasData)
    {
        var map = responce.Data.balance_changes.GroupByMaximumInDay().ToArray();
        Console.WriteLine(JsonConvert.SerializeObject(map));

    }
    else
    {
        Console.WriteLine("NO DATA");
    }

}
