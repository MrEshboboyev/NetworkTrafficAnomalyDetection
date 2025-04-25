using AnomalyDetection.Core.Entities;
using AnomalyDetection.Core.Repositories;
using AnomalyDetection.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AnomalyDetection.Data.Repositories;

// AlertRepository implementation
public class AlertRepository(ApplicationDbContext context) : IAlertRepository
{
    public async Task<AnomalyAlert> GetAlertByIdAsync(int id)
    {
        return await context.AnomalyAlerts
            .Include(a => a.RelatedLogs)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AnomalyAlert>> GetRecentAlertsAsync(int count)
    {
        return await context.AnomalyAlerts
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<AnomalyAlert> AddAlertAsync(AnomalyAlert alert)
    {
        await context.AnomalyAlerts.AddAsync(alert);
        await context.SaveChangesAsync();
        return alert;
    }

    public async Task<bool> UpdateAlertAsync(AnomalyAlert alert)
    {
        context.AnomalyAlerts.Update(alert);
        var saveResult = await context.SaveChangesAsync();
        return saveResult > 0;
    }
}
