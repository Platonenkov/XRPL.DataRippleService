namespace XRPL.DataRippleService.Exchanges;

public class DataRippleAccountBalancesResponse
{
    public string result { get; set; }
    public int ledger_index { get; set; }
    public string close_time { get; set; }
    public string limit { get; set; }
    public string marker { get; set; }
    public List<BalanceObject> balances { get; set; }
}