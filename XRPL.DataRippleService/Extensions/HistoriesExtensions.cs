using XRPL.DataRippleService.Exchanges;

namespace XRPL.DataRippleService.Extensions
{
    public static class HistoriesExtensions
    {
        public static RippleServiceCurrency CalculateProfit(this List<ExchangeObject> history, string Wallet, bool ForBaseAmount)
        {
            var first = history.FirstOrDefault();
            if (first is null)
                return null;

            double result;
            if (ForBaseAmount)
            {
                var sell = history.Where(c => c.seller == Wallet).Sum(c => c.base_amount);
                var buy = history.Where(c => c.buyer == Wallet).Sum(c => c.base_amount);
                result = sell - buy;
            }
            else
            {
                var sell = history.Where(c => c.seller == Wallet).Sum(c => c.counter_amount);
                var buy = history.Where(c => c.buyer == Wallet).Sum(c => c.counter_amount);
                result = sell - buy;
            }

            return FormCurrency(first, result, ForBaseAmount);
        }

        public static RippleServiceCurrency CalculateBuy(this List<ExchangeObject> history, string Wallet, bool ForBaseAmount)
        {
            var first = history.FirstOrDefault();
            if (first is null)
                return null;

            var result = ForBaseAmount
                ? history.Where(c => c.buyer == Wallet).Sum(c => c.base_amount)
                : history.Where(c => c.buyer == Wallet).Sum(c => c.counter_amount);
            return FormCurrency(first, result, ForBaseAmount);
        }

        public static RippleServiceCurrency CalculateSell(this List<ExchangeObject> history, string Wallet, bool ForBaseAmount)
        {
            var first = history.FirstOrDefault();
            if (first is null)
                return null;

            var result = ForBaseAmount
                ? history.Where(c => c.seller == Wallet).Sum(c => c.base_amount)
                : history.Where(c => c.seller == Wallet).Sum(c => c.counter_amount);
            return FormCurrency(first, result, ForBaseAmount);
        }
        private static RippleServiceCurrency FormCurrency(ExchangeObject one, double value, bool ForBaseAmount)
        {
            var currency_code = ForBaseAmount ? one.base_currency : one.counter_currency;
            var val = currency_code == "XRP" ? value / 0.000001 : value;
            return new RippleServiceCurrency()
            {
                CurrencyCode = currency_code,
                Issuer = ForBaseAmount ? one.base_issuer : one.counter_issuer,
                ValueAsNumber = Convert.ToDecimal(val)
            };
        }


    }
}
