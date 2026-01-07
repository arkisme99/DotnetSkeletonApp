using DotnetSkeleteonApp.Extensions;
using DotnetSkeletonApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// === Modular Configuration ===
builder.Services
    // .AddDatabaseAndMultiTenant(config)
    // .AddIdentityWithCookie()
    // .AddHangfireWithMySql(config)
    // .AddAuthorizationPolicies()
    .AddLocalizationSupport()
    .AddAppServices();

// builder.Services.AddHttpContextAccessor();
// Routing lowercase
builder.Services.AddSignalR();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true; // optional
});

builder.Services.AddHttpClient();

builder.Logging.AddConsole();
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// panggil seeder di sini
/* using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // 1️⃣ Jalankan migrasi untuk HOST
    await HostMigrationRunner.RunAsync(services);

    // Jalankan contoh tenant seeder // saat production di comment aja
    await TenantSeeder.SeedAsync(services);

    // Jalankan identity seeder untuk HOST
    await IdentitySeeder.SeedAsync(services);

    // 2️⃣ Jalankan migrasi untuk TENANT
    var tenantMigrator = services.GetRequiredService<TenantMigrationRunner>();

    await Task.Delay(2000); //delay sebentar agar aman migration tenantnya, 2 detik cukup

    await tenantMigrator.MigrateAllTenantsAsync();
} */

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
// app.UseRequestLocalization(locOptions.Value);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// app.UseHangfireDashboard("/hangfire-panel");

// app.MapHub<NotificationHub>("/hubs/notification");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=auth}/{action=login}/{id?}");

app.Run();
