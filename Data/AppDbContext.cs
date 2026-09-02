using Microsoft.EntityFrameworkCore;
using MinimalShop.Models;

namespace MinimalShop.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();
            entity.Property(c => c.Name).HasMaxLength(120);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(200);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.CustomerName).HasMaxLength(120);
            entity.Property(o => o.CustomerEmail).HasMaxLength(200);
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(i => i.ProductName).HasMaxLength(200);
            entity.Property(i => i.UnitPrice).HasPrecision(18, 2);
            entity.HasOne(i => i.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var electronics = new Category { Id = 1, Name = "لوازم دیجیتال", Description = "گجت‌ها و لوازم الکترونیکی" };
        var home = new Category { Id = 2, Name = "خانه و آشپزخانه", Description = "لوازم خانگی و آشپزخانه" };

        modelBuilder.Entity<Category>().HasData(electronics, home);

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "هدفون بی‌سیم", Description = "حذف نویز فعال", Price = 2_990_000m, Stock = 25, CategoryId = 1 },
            new Product { Id = 2, Name = "ساعت هوشمند", Description = "پایش سلامت و ورزش", Price = 5_490_000m, Stock = 15, CategoryId = 1 },
            new Product { Id = 3, Name = "قهوه‌ساز", Description = "برنامه‌ریزی خودکار", Price = 3_200_000m, Stock = 30, CategoryId = 2 },
            new Product { Id = 4, Name = "چراغ مطالعه", Description = "LED با نور قابل تنظیم", Price = 890_000m, Stock = 40, CategoryId = 2 }
        );
    }
}
