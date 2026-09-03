using Microsoft.EntityFrameworkCore;
using MinimalShop.Data;

namespace MinimalShop;

public class ShopSettings
{
    public const string SectionName = "Shop";
    public string StoreName { get; set; } = "MinimalShop";
}

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddShopServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ShopSettings>(builder.Configuration.GetSection(ShopSettings.SectionName));
        builder.Services.AddDbContext<ShopDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<Services.ShopService>();
        builder.Services.AddScoped<Services.CartService>();
        builder.Services.AddScoped<Services.AdminAuthService>();

        return builder;
    }
}
