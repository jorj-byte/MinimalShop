using Microsoft.EntityFrameworkCore;
using MinimalShop.Data;
using MinimalShop.Models;

namespace MinimalShop.Services;

public class ShopService(ShopDbContext db)
{
    public Task<List<Category>> GetActiveCategoriesAsync(CancellationToken ct = default) =>
        db.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct = default) =>
        db.Categories.OrderBy(c => c.Name).ToListAsync(ct);

    public Task<Category?> GetCategoryAsync(int id, CancellationToken ct = default) =>
        db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<List<Product>> GetActiveProductsAsync(int? categoryId = null, CancellationToken ct = default)
    {
        var query = db.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.Category.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        return query.OrderBy(p => p.Name).ToListAsync(ct);
    }

    public Task<List<Product>> GetAllProductsAsync(CancellationToken ct = default) =>
        db.Products.Include(p => p.Category).OrderBy(p => p.Name).ToListAsync(ct);

    public Task<Product?> GetProductAsync(int id, CancellationToken ct = default) =>
        db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Category> SaveCategoryAsync(Category category, CancellationToken ct = default)
    {
        category.Slug = Slugify(string.IsNullOrWhiteSpace(category.Slug) ? category.Name : category.Slug);

        if (category.Id == 0)
            db.Categories.Add(category);
        else
            db.Categories.Update(category);

        await db.SaveChangesAsync(ct);
        return category;
    }

    public async Task DeleteCategoryAsync(int id, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([id], ct)
            ?? throw new InvalidOperationException("Category not found.");

        var hasProducts = await db.Products.AnyAsync(p => p.CategoryId == id, ct);
        if (hasProducts)
            throw new InvalidOperationException("Remove or reassign products before deleting this category.");

        db.Categories.Remove(category);
        await db.SaveChangesAsync(ct);
    }

    public async Task<Product> SaveProductAsync(Product product, CancellationToken ct = default)
    {
        if (product.Id == 0)
            db.Products.Add(product);
        else
            db.Products.Update(product);

        await db.SaveChangesAsync(ct);
        return product;
    }

    public async Task DeleteProductAsync(int id, CancellationToken ct = default)
    {
        var product = await db.Products.FindAsync([id], ct)
            ?? throw new InvalidOperationException("Product not found.");

        db.Products.Remove(product);
        await db.SaveChangesAsync(ct);
    }

    public Task<List<Order>> GetOrdersAsync(CancellationToken ct = default) =>
        db.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public Task<Order?> GetOrderAsync(int id, CancellationToken ct = default) =>
        db.Orders.Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<Order> PlaceOrderAsync(
        string customerName,
        string customerEmail,
        string? customerPhone,
        string shippingAddress,
        IEnumerable<CartLine> cartLines,
        CancellationToken ct = default)
    {
        var lines = cartLines.ToList();
        if (lines.Count == 0)
            throw new InvalidOperationException("Cart is empty.");

        var productIds = lines.Select(l => l.ProductId).ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);

        foreach (var line in lines)
        {
            if (!products.TryGetValue(line.ProductId, out var product))
                throw new InvalidOperationException($"Product {line.ProductId} is no longer available.");

            if (product.Stock < line.Quantity)
                throw new InvalidOperationException($"Not enough stock for {product.Name}.");
        }

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerName = customerName.Trim(),
            CustomerEmail = customerEmail.Trim(),
            CustomerPhone = string.IsNullOrWhiteSpace(customerPhone) ? null : customerPhone.Trim(),
            ShippingAddress = shippingAddress.Trim(),
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        decimal total = 0;
        foreach (var line in lines)
        {
            var product = products[line.ProductId];
            product.Stock -= line.Quantity;

            var item = new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = line.Quantity
            };

            total += item.LineTotal;
            order.Items.Add(item);
        }

        order.Total = total;
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
        return order;
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status, CancellationToken ct = default)
    {
        var order = await db.Orders.FindAsync([orderId], ct)
            ?? throw new InvalidOperationException("Order not found.");

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    private static string GenerateOrderNumber() =>
        $"MS-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";

    private static string Slugify(string value) =>
        string.Join('-', value.Trim().ToLowerInvariant()
            .Split([' ', '-', '_'], StringSplitOptions.RemoveEmptyEntries));
}
