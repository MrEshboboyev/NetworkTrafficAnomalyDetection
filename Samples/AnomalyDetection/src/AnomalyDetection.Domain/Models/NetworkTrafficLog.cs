namespace AnomalyDetection.Domain.Models;

public class NetworkTrafficLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string SourceIp { get; set; }
    public string DestinationIp { get; set; }
    public int Port { get; set; }
    public string Protocol { get; set; }
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    public int ConnectionDuration { get; set; }
    public string UserAgent { get; set; }
    public bool IsAnomaly { get; set; }
    public double AnomalyScore { get; set; }
}