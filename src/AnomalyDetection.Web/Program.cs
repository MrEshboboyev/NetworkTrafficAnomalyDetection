using AnomalyDetection.Core.Entities;
using AnomalyDetection.Core.Repositories;
using AnomalyDetection.Data.Context;
using AnomalyDetection.Data.Repositories;
using AnomalyDetection.Data.Seeding;
using AnomalyDetection.Web.Background;
using AnomalyDetection.Web.Services.AlertSystem;
using AnomalyDetection.Web.Services.DataIngestion;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DatabaseConnection")));

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 10;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure application services
// Data ingestion services
builder.Services.AddTransient<ILogFileParser, CsvLogParser>();
// Add other log parsers as needed (JSON, PCAP, etc.)
builder.Services.AddTransient<LogParserFactory>();

// Feature engineering services
builder.Services.AddTransient<AnomalyDetection.Web.Services.FeatureEngineering.FeatureExtractor>();

// Machine learning services
builder.Services.AddTransient<AnomalyDetection.ML.Trainers.AnomalyDetectionTrainer>();

// Alert system services
builder.Services.AddTransient<AlertService>();
builder.Services.AddTransient<IAlertRepository, AlertRepository>();
builder.Services.AddTransient<IModelRepository, ModelRepository>();
builder.Services.AddTransient<INetworkTrafficRepository, NetworkTrafficRepository>();

// Background services for continuous monitoring
builder.Services.AddHostedService<TrafficMonitoringService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Data seeding can be added here if needed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed initial admin user
        await SeedData.SeedUsersAsync(userManager, roleManager);

        // Seed additional data (1000 records for AnomalyAlert, NetworkTrafficLog, and MLModel)
        await SeedData.SeedAnomalyDataAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
