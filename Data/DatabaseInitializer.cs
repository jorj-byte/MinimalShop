using Microsoft.EntityFrameworkCore;
using MinimalShop.Models;

namespace MinimalShop.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();

        await db.Database.MigrateAsync();

        if (await db.Categories.AnyAsync())
            return;

        var electronics = new Category
        {
            Name = "Electronics",
            Slug = "electronics",
            Description = "Gadgets and devices"
        };
        var home = new Category
        {
            Name = "Home",
            Slug = "home",
            Description = "Items for your home"
        };

        db.Categories.AddRange(electronics, home);
        await db.SaveChangesAsync();

        db.Products.AddRange(
            new Product
            {
                Name = "Wireless Headphones",
                Description = "Noise-cancelling over-ear headphones.",
                Price = 79.99m,
                Stock = 25,
                CategoryId = electronics.Id
            },
            new Product
            {
                Name = "USB-C Hub",
                Description = "7-in-1 adapter with HDMI and SD card reader.",
                Price = 34.50m,
                Stock = 40,
                CategoryId = electronics.Id
            },
            new Product
            {
                Name = "Ceramic Mug Set",
                Description = "Set of 4 mugs, dishwasher safe.",
                Price = 24.00m,
                Stock = 60,
                CategoryId = home.Id
            },
            new Product
            {
                Name = "Desk Lamp",
                Description = "Adjustable LED lamp with warm light.",
                Price = 29.99m,
                Stock = 18,
                CategoryId = home.Id
            });

        await db.SaveChangesAsync();
    }
}
