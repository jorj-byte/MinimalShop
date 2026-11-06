using MinimalShop;
using MinimalShop.Components;
using MinimalShop.Data;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.AddShopServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services);

app.UseForwardedHeaders();

// Serves files uploaded at runtime (e.g. /images/products/upload-*), which the
// build-time manifest of MapStaticAssets can never know about.
app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapAdminAuthEndpoints();
app.MapPaymentEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
