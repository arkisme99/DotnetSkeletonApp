using DotnetSkeletonApp.Extensions;
using DotnetSkeletonApp.Notifications;
using DotnetSkeletonApp.Seeders;
using Hangfire;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// === Modular Configuration ===
builder.Services
    .AddDatabase(config)
    .AddIdentityWithCookie()
    .AddHangfireWithMySql(config)
    .AddAuthorizationPolicies()
    .AddLocalizationSupport()
    .AddAppServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();
builder.Services.AddSingleton<JobNotificationFilter>();

// Routing lowercase
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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // Jalankan identity seeder
    await IdentitySeeder.SeedAsync(services);
    await SetupAppPermissionSeeder.SeedAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

var jobFilter = app.Services.GetRequiredService<JobNotificationFilter>();
GlobalJobFilters.Filters.Add(jobFilter);

app.UseHangfireDashboard("/hangfire-panel");

app.MapHub<JobNotificationHub>("/hubs/notification");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=auth}/{action=login}/{id?}");

app.Run();
