using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MinimalShop.Data;
using MinimalShop.Models;
using MinimalShop.Services;

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
        builder.Services.Configure<AdminAuthOptions>(builder.Configuration.GetSection(AdminAuthOptions.SectionName));

        builder.Services.AddDbContext<ShopDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddSingleton<AdminAuthService>();
        builder.Services.AddScoped<ShopService>();
        builder.Services.AddScoped<CartService>();

        builder.Services.Configure<PaymentSettings>(builder.Configuration.GetSection(PaymentSettings.SectionName));
        builder.Services.AddHttpClient<PaymentService>();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "MinimalShop.Admin";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = "/admin";
                options.AccessDeniedPath = "/admin";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return builder;
    }

    public static WebApplication MapAdminAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/account/login", async (HttpContext http, AdminAuthService auth) =>
        {
            var form = await http.Request.ReadFormAsync();
            var password = form["password"].ToString();
            var clientKey = http.Connection.RemoteIpAddress?.ToString()
                            ?? http.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? "unknown";

            var result = auth.Validate(password, clientKey);
            if (!result.Succeeded)
            {
                var error = Uri.EscapeDataString(result.Error ?? "Login failed");
                return Results.Redirect($"/admin?error={error}");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "admin"),
                new(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await http.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            return Results.Redirect("/admin/orders");
        }).AllowAnonymous().DisableAntiforgery();

        app.MapPost("/account/logout", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/admin");
        }).RequireAuthorization("AdminOnly").DisableAntiforgery();

        return app;
    }

    public static WebApplication MapPaymentEndpoints(this WebApplication app)
    {
        // Start the Zarinpal payment for an order and redirect the browser to the gateway.
        app.MapGet("/payment/start/{orderId:int}", async (
            int orderId,
            ShopService shop,
            PaymentService payment,
            HttpContext http,
            CancellationToken ct) =>
        {
            var order = await shop.GetOrderAsync(orderId, ct);
            if (order is null)
                return Results.Redirect("/shop");

            if (order.PaymentRefId is not null || order.Status is not OrderStatus.Pending)
                return Results.Redirect($"/order/{orderId}");

            var callback = $"{http.Request.Scheme}://{http.Request.Host}/payment/callback";
            var result = await payment.RequestPaymentAsync(
                order.Id, order.Total, $"پرداخت سفارش {order.OrderNumber}", callback, ct);

            if (!result.Succeeded)
                return Results.Redirect($"/order/{orderId}?payment=error&msg={Uri.EscapeDataString(result.Error ?? "")}");

            order.PaymentAuthority = result.Authority;
            order.UpdatedAt = DateTime.UtcNow;
            await shop.SaveOrderAuthorityAsync(order.Id, result.Authority!, ct);

            return Results.Redirect(result.RedirectUrl!);
        }).ExcludeFromDescription();

        // Gateway return point: verify the payment and show the receipt.
        app.MapGet("/payment/callback", async (
            HttpContext http,
            ShopService shop,
            PaymentService payment,
            CancellationToken ct) =>
        {
            var authority = http.Request.Query["Authority"].ToString();
            var status = http.Request.Query["Status"].ToString();

            var order = string.IsNullOrWhiteSpace(authority)
                ? null
                : await shop.GetOrderByAuthorityAsync(authority, ct);

            if (order is null)
                return Results.Redirect("/cart?payment=invalid");

            if (!string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase))
                return Results.Redirect($"/order/{order.Id}?payment=nok");

            var verify = await payment.VerifyPaymentAsync(authority, order.Total, ct);
            if (!verify.Succeeded)
                return Results.Redirect($"/order/{order.Id}?payment=error&msg={Uri.EscapeDataString(verify.Error ?? "")}");

            await shop.MarkOrderPaidAsync(order.Id, verify.RefId ?? 0, ct);
            return Results.Redirect($"/order/{order.Id}?payment=ok");
        }).ExcludeFromDescription();

        return app;
    }
}
