using XRPL.DataRippleService.Enums;
using XRPL.DataRippleService.Exchanges;

namespace XRPL.DataRippleService.Extensions;

public static class BalanceChangeExtensions
{
    public static IEnumerable<BalanceSnapshot>? GroupByMaximumInDay(this IEnumerable<BalanceChangeData> data) =>
        data?.GroupBy(v => v.executed_time.Value.Date)
           .Select(c =>
            {
                var first = c.FirstOrDefault();
                if (first is null)
                    return null;

                return new BalanceSnapshot
                {
                    Currency = new RippleServiceCurrency()
                    {
                        CurrencyCode = first.currency,
                        Issuer = first.counterparty,
                        ValueAsNumber = c.Max(v => v.final_balance)
                    },
                    Date = c.Key
                };
            })
           .ToArray()!;
    public static IEnumerable<BalanceChangeData> Select(this IEnumerable<BalanceChangeData> data, ChangeTypeEnum Type)
        => data is null ? Array.Empty<BalanceChangeData>() : Type switch
        {
            ChangeTypeEnum.None => data,
            ChangeTypeEnum.transaction_cost => data.Where(c => c.ChangeType == ChangeTypeEnum.transaction_cost),
            ChangeTypeEnum.payment_destination => data.Where(c => c.ChangeType == ChangeTypeEnum.payment_destination),
            ChangeTypeEnum.payment_source => data.Where(c => c.ChangeType == ChangeTypeEnum.payment_source),
            ChangeTypeEnum.exchange => data.Where(c => c.ChangeType == ChangeTypeEnum.exchange),
            ChangeTypeEnum.intermediary => data.Where(c => c.ChangeType == ChangeTypeEnum.intermediary),
            _ => throw new ArgumentOutOfRangeException()
        };
    public static IEnumerable<BalanceChangeData> Select(this IEnumerable<BalanceChangeData> data, BalanceChangeResultEnum Type)
        => data is null
            ? Array.Empty<BalanceChangeData>()
            : Type switch
            {
                BalanceChangeResultEnum.none => data,
                BalanceChangeResultEnum.Sold => data.Where(c => c.ChangeType == ChangeTypeEnum.exchange).Where(c => c.amount_change < 0),
                BalanceChangeResultEnum.Bought => data.Where(c => c.ChangeType == ChangeTypeEnum.exchange).Where(c => c.amount_change > 0),
                BalanceChangeResultEnum.Sent => data.Where(c => c.ChangeType == ChangeTypeEnum.payment_source),
                BalanceChangeResultEnum.Receive => data.Where(c => c.ChangeType == ChangeTypeEnum.payment_destination),
                _ => throw new ArgumentOutOfRangeException()
            };
    public static double Calculate(this IEnumerable<BalanceChangeData> data, ChangeTypeEnum Type)
        => data is null ? 0 : Type switch
        {
            ChangeTypeEnum.None => data.Sum(c => c.amount_change),
            ChangeTypeEnum.transaction_cost => data.Where(c => c.ChangeType == ChangeTypeEnum.transaction_cost).Sum(c => c.amount_change),
            ChangeTypeEnum.payment_destination => data.Where(c => c.ChangeType == ChangeTypeEnum.payment_destination).Sum(c => c.amount_change),
            ChangeTypeEnum.payment_source => data.Where(c => c.ChangeType == ChangeTypeEnum.payment_source).Sum(c => c.amount_change),
            ChangeTypeEnum.exchange => data.Where(c => c.ChangeType == ChangeTypeEnum.exchange).Sum(c => c.amount_change),
            ChangeTypeEnum.intermediary => data.Where(c => c.ChangeType == ChangeTypeEnum.intermediary).Sum(c => c.amount_change),
            _ => throw new ArgumentOutOfRangeException()
        };
    public static double Calculate(this IEnumerable<BalanceChangeData> data, BalanceChangeResultEnum Type)
        => data is null ? 0 : Type switch
        {
            BalanceChangeResultEnum.none => data.Sum(c => c.amount_change),
            BalanceChangeResultEnum.Sold => data.Where(c => c.ChangeType == ChangeTypeEnum.exchange).Where(c => c.amount_change < 0).Sum(c => c.amount_change),
            BalanceChangeResultEnum.Bought => data.Where(c => c.ChangeType == ChangeTypeEnum.exchange).Where(c => c.amount_change > 0).Sum(c => c.amount_change),
            BalanceChangeResultEnum.Sent => data.Where(c => c.ChangeType == ChangeTypeEnum.payment_source).Sum(c => c.amount_change),
            BalanceChangeResultEnum.Receive => data.Where(c => c.ChangeType == ChangeTypeEnum.payment_destination).Sum(c => c.amount_change),
            _ => throw new ArgumentOutOfRangeException()
        };

}