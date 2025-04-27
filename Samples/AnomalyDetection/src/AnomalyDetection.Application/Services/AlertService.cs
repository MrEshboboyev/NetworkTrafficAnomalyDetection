using AnomalyDetection.Domain.Interfaces;
using AnomalyDetection.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalyDetection.Application.Services;

public class AlertService
{
    private readonly ILogger<AlertService> _logger;
    private readonly IRepository<NetworkTrafficLog> _repository;

    // In a real system, this might be connected to a notification service
    public event EventHandler<NetworkTrafficLog> AlertGenerated;

    public AlertService(
        ILogger<AlertService> logger,
        IRepository<NetworkTrafficLog> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task CreateAlertAsync(NetworkTrafficLog log)
    {
        _logger.LogWarning("ALERT: Anomaly detected in traffic log ID {logId} with score {score}",
            log.Id, log.AnomalyScore);

        // In a real system, you would save the alert to a database
        // and potentially trigger notifications

        // Trigger the alert event
        AlertGenerated?.Invoke(this, log);

        await Task.CompletedTask;
    }

    public async Task<IEnumerable<NetworkTrafficLog>> GetRecentAlertsAsync(int count = 10)
    {
        // Get the most recent anomalies
        var recentAlerts = await _repository.FindAsync(log => log.IsAnomaly);
        return recentAlerts;
    }
}