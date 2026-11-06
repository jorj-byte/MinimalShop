using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MinimalShop.Data;
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
}
