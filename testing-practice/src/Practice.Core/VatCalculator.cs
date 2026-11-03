namespace Practice.Core;

public static class VatCalculator
{
    public const decimal Rate = 0.16m;

    public static decimal AddVat(decimal net)
    {
        if (net < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(net), "Net amount cannot be negative.");
        }

        return Math.Round(net * (1 + Rate), 2, MidpointRounding.AwayFromZero);
    }
}
