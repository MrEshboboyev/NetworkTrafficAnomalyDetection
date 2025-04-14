namespace NetworkTrafficAnomalyDetection.Models;

public class NetworkTraffic
{
    public int Id { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public int PacketLength { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsAnomaly { get; set; }  // Label for anomaly detection
}
