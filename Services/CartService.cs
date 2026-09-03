namespace MinimalShop.Services;

public class CartLine
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => Price * Quantity;
}

public class CartService
{
    private readonly Dictionary<int, CartLine> _lines = [];

    public event Action? OnChange;

    public IReadOnlyCollection<CartLine> Lines => _lines.Values;
    public int ItemCount => _lines.Values.Sum(l => l.Quantity);
    public decimal Subtotal => _lines.Values.Sum(l => l.LineTotal);

    public void Add(int productId, string name, decimal price, int quantity = 1)
    {
        if (_lines.TryGetValue(productId, out var line))
            line.Quantity += quantity;
        else
            _lines[productId] = new CartLine { ProductId = productId, Name = name, Price = price, Quantity = quantity };

        Notify();
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        if (!_lines.TryGetValue(productId, out var line))
            return;

        if (quantity <= 0)
            _lines.Remove(productId);
        else
            line.Quantity = quantity;

        Notify();
    }

    public void Remove(int productId)
    {
        if (_lines.Remove(productId))
            Notify();
    }

    public void Clear()
    {
        _lines.Clear();
        Notify();
    }

    private void Notify() => OnChange?.Invoke();
}
