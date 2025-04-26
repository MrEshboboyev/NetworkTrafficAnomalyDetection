using AnomalyDetection.Core.Entities;
using AnomalyDetection.Core.Repositories;
using AnomalyDetection.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AnomalyDetection.Data.Repositories;

public class NetworkTrafficRepository(ApplicationDbContext context) : INetworkTrafficRepository
{
    public async Task<IEnumerable<NetworkTrafficLog>> GetTrainingDataAsync()
    {
        //// Get data from the last 30 days, excluding the most recent 2 days (for test data)
        //var cutoffDate = DateTime.UtcNow.AddDays(-2);
        //var startDate = DateTime.UtcNow.AddDays(-30);

        return await context.NetworkTrafficLogs
            //.Where(log => log.Timestamp >= startDate && log.Timestamp < cutoffDate)
            .OrderBy(log => log.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<NetworkTrafficLog>> GetTestDataAsync()
    {
        // Get the most recent 2 days of data for testing
        var cutoffDate = DateTime.UtcNow.AddDays(-2);

        return await context.NetworkTrafficLogs
            .Where(log => log.Timestamp >= cutoffDate)
            .OrderBy(log => log.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<NetworkTrafficLog>> GetAnomalousTrafficAsync()
    {
        return await context.NetworkTrafficLogs
            .Where(log => log.IsAnomaly)
            .OrderByDescending(log => log.AnomalyScore)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await context.NetworkTrafficLogs.CountAsync();
    }
}
