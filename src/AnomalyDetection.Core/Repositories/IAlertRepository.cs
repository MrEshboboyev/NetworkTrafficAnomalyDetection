using AnomalyDetection.Core.Entities;

namespace AnomalyDetection.Core.Repositories;

public interface IAlertRepository
{
    Task<AnomalyAlert> GetAlertByIdAsync(int id);
    Task<IEnumerable<AnomalyAlert>> GetRecentAlertsAsync(int count);
    Task<AnomalyAlert> AddAlertAsync(AnomalyAlert alert);
    Task<bool> UpdateAlertAsync(AnomalyAlert alert);
}
