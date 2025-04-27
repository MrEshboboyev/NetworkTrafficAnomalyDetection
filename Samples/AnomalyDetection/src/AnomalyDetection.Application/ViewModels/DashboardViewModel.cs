using AnomalyDetection.Domain.Models;

namespace AnomalyDetection.Application.ViewModels;

public class DashboardViewModel
{
    public IEnumerable<NetworkTrafficLog> RecentAlerts { get; set; }
    public int TotalTrafficLogs { get; set; }
    public int TotalAnomalies { get; set; }
    public double AnomalyPercentage { get; set; }
}

public class ErrorViewModel
{
    public string RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}