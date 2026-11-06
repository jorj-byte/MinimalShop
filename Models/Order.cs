namespace MinimalShop.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Online payment data (Zarinpal)
    public string? PaymentAuthority { get; set; }
    public long? PaymentRefId { get; set; }
    public DateTime? PaidAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}
