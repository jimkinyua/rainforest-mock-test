namespace Practice.Core;

public interface IExchangeRates
{
    decimal GetRate(string from, string to);
}

public class Converter(IExchangeRates rates)
{
    public decimal Convert(decimal amount, string from, string to)
    {
        if (string.Equals(from, to, StringComparison.OrdinalIgnoreCase))
        {
            return amount;
        }

        return Math.Round(amount * rates.GetRate(from, to), 2, MidpointRounding.AwayFromZero);
    }
}
