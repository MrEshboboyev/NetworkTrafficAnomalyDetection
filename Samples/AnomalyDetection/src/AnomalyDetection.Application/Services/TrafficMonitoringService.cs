using AnomalyDetection.Domain.Interfaces;
using AnomalyDetection.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalyDetection.Application.Services;

public class TrafficMonitoringService : BackgroundService
{
    private readonly ILogger<TrafficMonitoringService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _processingInterval = TimeSpan.FromMinutes(5);

    public TrafficMonitoringService(
        ILogger<TrafficMonitoringService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Traffic Monitoring Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Processing traffic logs at: {time}", DateTimeOffset.Now);

            try
            {
                await ProcessTrafficLogsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing traffic logs");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Traffic Monitoring Service is stopping.");
    }

    private async Task ProcessTrafficLogsAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IRepository<NetworkTrafficLog>>();
            var analyzer = scope.ServiceProvider.GetRequiredService<ITrafficAnalyzer>();
            var alertService = scope.ServiceProvider.GetRequiredService<AlertService>();

            // Get unprocessed logs
            var unprocessedLogs = await repository.FindAsync(log => log.AnomalyScore == 0);

            if (!unprocessedLogs.Any())
            {
                _logger.LogInformation("No unprocessed logs found.");
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

            _logger.LogInformation("Processed {count} logs. Found {anomalyCount} anomalies.",
                analyzedLogs.Count(), analyzedLogs.Count(l => l.IsAnomaly));
        }
    }
}