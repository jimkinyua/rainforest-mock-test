namespace Practice.Core;

public record CartLine(string Sku, decimal UnitPrice, int Quantity)
{
    public decimal LineTotal => UnitPrice * Quantity;
}

public class ShoppingCart
{
    private readonly List<CartLine> _lines = [];
    private decimal _discountRate;

    public IReadOnlyList<CartLine> Lines => _lines.AsReadOnly();

    public decimal Subtotal => _lines.Sum(l => l.LineTotal);

    public decimal Total => Math.Round(Subtotal * (1 - _discountRate), 2, MidpointRounding.AwayFromZero);

    public void AddItem(string sku, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be at least 1.");
        }

        var existing = _lines.FirstOrDefault(l => l.Sku == sku);
        if (existing is not null)
        {
            _lines.Remove(existing);
            _lines.Add(existing with { Quantity = existing.Quantity + quantity });
            return;
        }

        _lines.Add(new CartLine(sku, unitPrice, quantity));
    }

    public void ApplyDiscount(string code)
    {
        if (code != "SAVE10")
        {
            throw new ArgumentException("Unknown discount code.", nameof(code));
        }

        if (Subtotal > 1000m)
        {
            _discountRate = 0.10m;
        }
    }
}
