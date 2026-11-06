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
            Name = "لوازم جانبی",
            Slug = "electronics",
            Description = "هدفون، شارژر و اکسسوری موبایل"
        };
        var home = new Category
        {
            Name = "خانگی",
            Slug = "home",
            Description = "کالاهای مورد نیاز خانه"
        };

        db.Categories.AddRange(electronics, home);
        await db.SaveChangesAsync();

        db.Products.AddRange(
            new Product
            {
                Name = "هدفون بی‌سیم سونی WH-CH520",
                Description = "هدفون روگوشی با بلوتوث ۵.۲ و عمر باتری تا ۵۰ ساعت؛ مناسب گوش دادن طولانی",
                Price = 2_450_000m,
                Stock = 25,
                CategoryId = electronics.Id,
                ImageUrl = "/images/products/sony-headphone-whch520.jpg",
                Color = "مشکی",
                WeightGrams = 150,
                LengthCm = 19, WidthCm = 17, HeightCm = 8
            },
            new Product
            {
                Name = "شارژر دیواری ۶۵ وات اسپارک",
                Description = "شارژر سریع با سه پورت شامل دو USB-C و یک USB-A؛ پشتیبانی از شارژ هم‌زمان لپ‌تاپ و موبایل",
                Price = 890_000m,
                Stock = 40,
                CategoryId = electronics.Id,
                ImageUrl = "/images/products/spark-65w-charger.jpg",
                Color = "خاکستری",
                WeightGrams = 95,
                LengthCm = 11, WidthCm = 4, HeightCm = 2
            },
            new Product
            {
                Name = "کابل شارژ تایپ-سی ۱ متر",
                Description = "کابل بافته نایلونی با جریان ۳ آمپر و پشتیبانی از انتقال سریع داده",
                Price = 190_000m,
                Stock = 120,
                CategoryId = electronics.Id,
                ImageUrl = "/images/products/typec-cable-1m.jpg",
                Color = "سرمه‌ای",
                WeightGrams = 35,
                LengthCm = 100, WidthCm = 2, HeightCm = 1
            },
            new Product
            {
                Name = "پاوربانک انکر ۲۰ هزار میلی‌آمپر",
                Description = "پاوربانک با دو خروجی USB-C و USB-A و شارژ سریع ۲۲.۵ وات",
                Price = 1_390_000m,
                Stock = 30,
                CategoryId = electronics.Id,
                ImageUrl = "/images/products/anker-powerbank-20000.jpg",
                Color = "آبی",
                WeightGrams = 440,
                LengthCm = 13, WidthCm = 6, HeightCm = 3
            },
            new Product
            {
                Name = "ساعت هوشمند سامسونگ گلکسی فیت ۳",
                Description = "ساعت هوشمند با نمایشگر AMOLED ۱.۶ اینچی و عمر باتری تا ۱۴ روز",
                Price = 3_250_000m,
                Stock = 18,
                CategoryId = electronics.Id,
                ImageUrl = "/images/products/galaxyfit3-watch.jpg",
                Color = "نقره‌ای",
                WeightGrams = 37,
                LengthCm = 5, WidthCm = 4, HeightCm = 1
            },
            new Product
            {
                Name = "سرویس فنجان چینی ۴ عددی",
                Description = "سرویس فنجان چینی با طرح کلاسیک و نعلبکی؛ مناسب پذیرایی و کادو",
                Price = 680_000m,
                Stock = 60,
                CategoryId = home.Id,
                ImageUrl = "/images/products/porcelain-tea-set-4.jpg",
                Color = "سفید",
                WeightGrams = 950,
                LengthCm = 25, WidthCm = 15, HeightCm = 12
            },
            new Product
            {
                Name = "ترازوی آشپزخانه شیائومی",
                Description = "ترازوی دیجیتال آشپزخانه با ظرفیت ۱۰ کیلوگرم و نمایشگر LED",
                Price = 740_000m,
                Stock = 20,
                CategoryId = home.Id,
                ImageUrl = "/images/products/xiaomi-kitchen-scale.jpg",
                Color = "سفید",
                WeightGrams = 300,
                LengthCm = 18, WidthCm = 13, HeightCm = 4
            },
            new Product
            {
                Name = "چراغ مطالعه رومیزی مدل ML-2170",
                Description = "چراغ مطالعه با بازوی تاشو، بدنه فلزی و شدت نور تنظیم‌پذیر",
                Price = 550_000m,
                Stock = 18,
                CategoryId = home.Id,
                ImageUrl = "/images/products/ml2170-desk-lamp.jpg",
                Color = "نقره‌ای",
                WeightGrams = 850,
                LengthCm = 35, WidthCm = 15, HeightCm = 45
            });

        await db.SaveChangesAsync();
    }
}
