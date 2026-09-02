namespace MinimalShop.Services;

public class CartItem
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => Price * Quantity;
}

public class CartService
{
    private readonly List<CartItem> _items = [];

    public IReadOnlyList<CartItem> Items => _items;
    public decimal Total => _items.Sum(i => i.LineTotal);
    public int ItemCount => _items.Sum(i => i.Quantity);

    public void Add(int productId, string name, decimal price, int quantity = 1)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.Quantity += quantity;
            return;
        }

        _items.Add(new CartItem
        {
            ProductId = productId,
            Name = name,
            Price = price,
            Quantity = quantity
        });
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
        {
            return;
        }

        if (quantity <= 0)
        {
            _items.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }
    }

    public void Remove(int productId) =>
        _items.RemoveAll(i => i.ProductId == productId);

    public void Clear() => _items.Clear();
}
