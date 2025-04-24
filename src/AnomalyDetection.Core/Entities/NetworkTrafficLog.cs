namespace AnomalyDetection.Core.Entities;

public class NetworkTrafficLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string SourceIp { get; set; }
    public string DestinationIp { get; set; }
    public int SourcePort { get; set; }
    public int DestinationPort { get; set; }
    public string Protocol { get; set; }  // TCP, UDP, ICMP, etc.
    public long PacketSize { get; set; } // in bytes
    public double Duration { get; set; } // in seconds
    public int PacketCount { get; set; }
    public string Flags { get; set; }    // TCP flags if applicable
    public bool IsAnomaly { get; set; }  // Detected anomaly flag
    public double AnomalyScore { get; set; } // Score indicating anomaly severity
    public string AnomalyType { get; set; } // Type of anomaly if detected
}
