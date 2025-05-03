using AnomalyDetection.Domain.Models;

namespace AnomalyDetection.Web.Models;

public class DashboardViewModel
{
    public IEnumerable<NetworkTrafficLog> RecentAlerts { get; set; }
    public int TotalTrafficLogs { get; set; }
    public int TotalAnomalies { get; set; }
    public double AnomalyPercentage { get; set; }
}
