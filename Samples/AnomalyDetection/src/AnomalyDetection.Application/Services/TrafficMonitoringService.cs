using AnomalyDetection.Domain.Interfaces;
using AnomalyDetection.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AnomalyDetection.Application.Services;

public class TrafficMonitoringService(
    ILogger<TrafficMonitoringService> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    private readonly TimeSpan _processingInterval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Traffic Monitoring Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Processing traffic logs at: {time}", DateTimeOffset.Now);

            try
            {
                await ProcessTrafficLogsAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing traffic logs");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        logger.LogInformation("Traffic Monitoring Service is stopping.");
    }

    private async Task ProcessTrafficLogsAsync()
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IRepository<NetworkTrafficLog>>();
            var analyzer = scope.ServiceProvider.GetRequiredService<ITrafficAnalyzer>();
            var alertService = scope.ServiceProvider.GetRequiredService<AlertService>();

            // Get unprocessed logs
            var unprocessedLogs = await repository.FindAsync(log => log.AnomalyScore == 0);

            if (!unprocessedLogs.Any())
            {
                logger.LogInformation("No unprocessed logs found.");
                return;
            }

            // Analyze logs for anomalies
            var analyzedLogs = await analyzer.AnalyzeTrafficAsync(unprocessedLogs);

            // Update logs in database
            foreach (var log in analyzedLogs)
            {
                await repository.UpdateAsync(log);

                // Generate alerts for anomalies
                if (log.IsAnomaly)
                {
                    await alertService.CreateAlertAsync(log);
                }
            }

            logger.LogInformation("Processed {count} logs. Found {anomalyCount} anomalies.",
                analyzedLogs.Count(), analyzedLogs.Count(l => l.IsAnomaly));
        }
    }
}
