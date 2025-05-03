using AnomalyDetection.Application.Services;
using AnomalyDetection.Domain.Interfaces;
using AnomalyDetection.Domain.Models;
using AnomalyDetection.Infrastructure.Data;
using AnomalyDetection.Infrastructure.Data.Repositories;
using AnomalyDetection.Infrastructure.ML;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IRepository<NetworkTrafficLog>, TrafficLogRepository>();

// Register ML services
builder.Services.AddSingleton<ITrafficAnalyzer, AnomalyDetector>();

// Register application services
builder.Services.AddScoped<AlertService>();
builder.Services.AddHostedService<TrafficMonitoringService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();