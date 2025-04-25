namespace AnomalyDetection.Web.Models;

public class TrafficSummaryViewModel
{
    public long TotalPackets { get; set; }
    public double AveragePacketSize { get; set; }
    public Dictionary<string, int> TopSourceIps { get; set; }
    public Dictionary<int, int> TopDestinationPorts { get; set; }
}
