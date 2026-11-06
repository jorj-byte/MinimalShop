namespace MinimalShop.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Web path of the product photo under /images/products (null = placeholder).</summary>
    public string? ImageUrl { get; set; }

    // Product specifications (all optional until the seller fills them in).
    public string? Color { get; set; }
    public int? WeightGrams { get; set; }
    public int? LengthCm { get; set; }
    public int? WidthCm { get; set; }
    public int? HeightCm { get; set; }

    public Category Category { get; set; } = null!;
}
