using AnomalyDetection.Core.Entities;
using AnomalyDetection.Data.Context;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AnomalyDetection.Data.Seeding;

public static class SeedData
{
    public static async Task SeedUsersAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        string[] roles = ["Admin", "Analyst", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user if it doesn't exist
        var adminEmail = "admin@anomalydetection.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123456");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    public static async Task SeedAnomalyDataAsync(ApplicationDbContext context)
    {
        var randomizer = new Randomizer();

        // Seed Anomaly Alerts (1000)
        if (!await context.AnomalyAlerts.AnyAsync())
        {
            var anomalyAlertFaker = new Faker<AnomalyAlert>()
                .RuleFor(a => a.Timestamp, f => f.Date.Recent().ToUniversalTime())  // Convert to UTC
                .RuleFor(a => a.AlertType, f => f.PickRandom("DDoS", "Unauthorized Access", "Data Exfiltration"))
                .RuleFor(a => a.Description, f => f.Lorem.Sentence())
                .RuleFor(a => a.SeverityLevel, f => f.PickRandom("High", "Medium", "Low"))
                .RuleFor(a => a.ConfidenceScore, f => f.Random.Double(0.5, 1.0))
                .RuleFor(a => a.IsAcknowledged, f => f.Random.Bool())
                .RuleFor(a => a.AcknowledgedAt, f => f.Date.Recent().ToUniversalTime())  // Convert to UTC
                .RuleFor(a => a.AcknowledgedBy, f => f.Name.FullName())
                .RuleFor(a => a.RelatedLogs, f => new List<NetworkTrafficLog>());

            var anomalyAlerts = anomalyAlertFaker.Generate(1000);
            await context.AnomalyAlerts.AddRangeAsync(anomalyAlerts);
            await context.SaveChangesAsync();
        }

        // Seed Network Traffic Logs (1000)
        if (!await context.NetworkTrafficLogs.AnyAsync())
        {
            var networkTrafficLogFaker = new Faker<NetworkTrafficLog>()
                .RuleFor(l => l.Timestamp, f => f.Date.Recent().ToUniversalTime())  // Convert to UTC
                .RuleFor(l => l.SourceIp, f => f.Internet.Ip())
                .RuleFor(l => l.DestinationIp, f => f.Internet.Ip())
                .RuleFor(l => l.SourcePort, f => f.Random.Int(1, 65535))
                .RuleFor(l => l.DestinationPort, f => f.Random.Int(1, 65535))
                .RuleFor(l => l.Protocol, f => f.PickRandom("TCP", "UDP", "ICMP"))
                .RuleFor(l => l.PacketSize, f => f.Random.Long(100, 1500))
                .RuleFor(l => l.Duration, f => f.Random.Double(0.5, 60))
                .RuleFor(l => l.PacketCount, f => f.Random.Int(10, 500))
                .RuleFor(l => l.Flags, f => f.Lorem.Word())
                .RuleFor(l => l.IsAnomaly, f => f.Random.Bool())
                .RuleFor(l => l.AnomalyScore, f => f.Random.Double(0.0, 1.0))
                .RuleFor(l => l.AnomalyType, f => f.PickRandom("Port Scan", "Syn Flood", "Anomaly Detected", null));

            var networkTrafficLogs = networkTrafficLogFaker.Generate(1000);
            await context.NetworkTrafficLogs.AddRangeAsync(networkTrafficLogs);
            await context.SaveChangesAsync();
        }

        // Seed ML Models (1000)
        if (!await context.MLModels.AnyAsync())
        {
            var mlModelFaker = new Faker<MLModel>()
                .RuleFor(m => m.Name, f => f.Lorem.Word())
                .RuleFor(m => m.Description, f => f.Lorem.Sentence())
                .RuleFor(m => m.ModelType, f => f.PickRandom("IsolationForest", "PCA", "KMeans"))
                .RuleFor(m => m.CreatedAt, f => f.Date.Recent(100).ToUniversalTime())  // Convert to UTC
                .RuleFor(m => m.LastTrainedAt, f => f.Date.Recent(30).ToUniversalTime())  // Convert to UTC
                .RuleFor(m => m.LastEvaluatedAt, f => f.Date.Recent(7).ToUniversalTime())  // Convert to UTC
                .RuleFor(m => m.FilePath, f => $"/models/{f.Lorem.Word()}_model.model")
                .RuleFor(m => m.TrainingParameters, f => $"n_estimators={f.Random.Int(50, 200)}")
                .RuleFor(m => m.NumberOfFeatures, f => f.Random.Int(10, 100))
                .RuleFor(m => m.TrainingDataSize, f => f.Random.Int(50000, 500000))
                .RuleFor(m => m.Accuracy, f => f.Random.Double(0.8, 1.0))
                .RuleFor(m => m.Precision, f => f.Random.Double(0.7, 1.0))
                .RuleFor(m => m.Recall, f => f.Random.Double(0.7, 1.0))
                .RuleFor(m => m.F1Score, f => f.Random.Double(0.7, 1.0))
                .RuleFor(m => m.DecisionThreshold, f => f.Random.Float(0.5f, 1.0f))
                .RuleFor(m => m.Version, f => "1.0")
                .RuleFor(m => m.IsProduction, f => f.Random.Bool())
                .RuleFor(m => m.Metadata, f => new Dictionary<string, string>
                {
                { "TrainingDataset", f.Lorem.Word() },
                { "ModelCreator", f.Name.FullName() }
                });

            var mlModels = mlModelFaker.Generate(1000);
            await context.MLModels.AddRangeAsync(mlModels);
            await context.SaveChangesAsync();
        }
    }
}
