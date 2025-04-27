using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalyDetection.Application.Web.ViewModels;

public class TrafficSummaryViewModel
{
    public List<DailyTrafficSummary> DailyStats { get; set; }
}

public class DailyTrafficSummary
{
    public DateTime Date { get; set; }
    public int TotalTraffic { get; set; }
    public int AnomalyCount { get; set; }
    public double AnomalyPercentage => TotalTraffic > 0 ? (double)AnomalyCount / TotalTraffic * 100 : 0;
}