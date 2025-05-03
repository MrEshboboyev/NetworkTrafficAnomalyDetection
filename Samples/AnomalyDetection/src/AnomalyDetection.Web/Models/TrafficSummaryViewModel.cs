namespace AnomalyDetection.Web.Models;

public class TrafficSummaryViewModel
{
    public List<DailyTrafficSummary> DailyStats { get; set; }
}

public class DailyTrafficSummary
{
    public DateTime Date { get; set; }
    public int TotalTraffic { get; set; }
    public int AnomalyCount { get; set; }
    public double AnomalyPercentage => TotalTraffic > 0 
        ? (double)AnomalyCount / TotalTraffic * 100 
        : 0;
}
