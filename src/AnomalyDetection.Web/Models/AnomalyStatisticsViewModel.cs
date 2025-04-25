namespace AnomalyDetection.Web.Models;

public class AnomalyStatisticsViewModel
{
    public int TotalAnomalies { get; set; }
    public int HighSeverity { get; set; }
    public int MediumSeverity { get; set; }
    public int LowSeverity { get; set; }
    public Dictionary<string, int> AnomalyTypes { get; set; }
}
