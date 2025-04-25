using AnomalyDetection.Core.Entities;

namespace AnomalyDetection.Web.Models;

public class DashboardViewModel
{
    public List<AnomalyAlert> RecentAlerts { get; set; }
    public TrafficSummaryViewModel TrafficSummary { get; set; }
    public AnomalyStatisticsViewModel AnomalyStatistics { get; set; }
}
