using Microsoft.EntityFrameworkCore;
using MinimalShop.Data;
using MinimalShop.Models;
using MinimalShop.Services;

namespace MinimalShop.Services;

public class OrderService(AppDbContext db, CartService cart)
{
    public async Task<Order?> PlaceOrderAsync(string name, string email, string? phone, string? address)
    {
        if (cart.Items.Count == 0)
        {
            return null;
        }

        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var item in cart.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product) || product.Stock < item.Quantity)
            {
                return null;
            }
        }

        var order = new Order
        {
            CustomerName = name.Trim(),
            CustomerEmail = email.Trim(),
            CustomerPhone = phone?.Trim(),
            ShippingAddress = address?.Trim(),
            Status = OrderStatus.Pending,
            TotalAmount = cart.Total,
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Name,
                UnitPrice = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };

        foreach (var item in cart.Items)
        {
            products[item.ProductId].Stock -= item.Quantity;
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        cart.Clear();
        return order;
    }

    public Task<List<Order>> GetOrdersAsync() =>
        db.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public Task<Order?> GetOrderAsync(int id) =>
        db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<bool> UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
        {
            return false;
        }

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }
}
