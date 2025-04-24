// AnomalyDetection.Core/Entities/AnomalyAlert.cs
namespace AnomalyDetection.Core.Entities;

public class AnomalyAlert
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string AlertType { get; set; }      // DDoS, Unauthorized Access, Data Exfiltration, etc.
    public string Description { get; set; }
    public string SeverityLevel { get; set; }  // High, Medium, Low
    public double ConfidenceScore { get; set; } // Confidence level of the alert
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string AcknowledgedBy { get; set; }
    public List<NetworkTrafficLog> RelatedLogs { get; set; }
}
