using AnomalyDetection.Core.Entities;
using AnomalyDetection.Core.Repositories;

namespace AnomalyDetection.Web.Services.AlertSystem;

public class AlertService(IAlertRepository alertRepository)
{
    public async Task<AnomalyAlert> CreateAlertAsync(NetworkTrafficLog anomaly)
    {
        // Determine alert type based on anomaly characteristics
        string alertType = DetermineAlertType(anomaly);

        // Calculate severity based on anomaly score and other factors
        string severityLevel = CalculateSeverityLevel(anomaly.AnomalyScore);

        // Generate description based on type and context
        string description = GenerateAlertDescription(anomaly, alertType);

        // Create the alert
        var alert = new AnomalyAlert
        {
            Timestamp = DateTime.UtcNow,
            AlertType = alertType,
            Description = description,
            SeverityLevel = severityLevel,
            ConfidenceScore = anomaly.AnomalyScore,
            IsAcknowledged = false,
            RelatedLogs = new List<NetworkTrafficLog> { anomaly }
        };

        // Save alert
        await alertRepository.AddAlertAsync(alert);

        // Return the created alert
        return alert;
    }

    public async Task<List<AnomalyAlert>> CreateAlertsFromAnomaliesAsync(IEnumerable<NetworkTrafficLog> anomalies)
    {
        var alerts = new List<AnomalyAlert>();

        // Group similar anomalies to reduce alert fatigue
        var groupedAnomalies = GroupSimilarAnomalies(anomalies);

        foreach (var group in groupedAnomalies)
        {
            // Create an alert for each distinct group
            var mainAnomaly = group.OrderByDescending(a => a.AnomalyScore).First();

            var alert = await CreateAlertAsync(mainAnomaly);
            alert.RelatedLogs = group.ToList();

            // Update the alert in the repository
            await alertRepository.UpdateAlertAsync(alert);

            alerts.Add(alert);
        }

        return alerts;
    }

    public async Task<bool> AcknowledgeAlertAsync(int alertId, string username)
    {
        var alert = await alertRepository.GetAlertByIdAsync(alertId);

        if (alert == null)
        {
            return false;
        }

        alert.IsAcknowledged = true;
        alert.AcknowledgedAt = DateTime.UtcNow;
        alert.AcknowledgedBy = username;

        await alertRepository.UpdateAlertAsync(alert);

        return true;
    }

    #region Helper Methods
    private string DetermineAlertType(NetworkTrafficLog anomaly)
    {
        // Logic to determine the type of anomaly based on traffic characteristics

        // Example: DDoS detection
        if (anomaly.PacketCount > 1000 && anomaly.Duration < 1.0)
        {
            return "DDoS Attack";
        }

        // Example: Port scanning detection
        if (anomaly.DestinationPort < 1024 && anomaly.Protocol == "TCP" && anomaly.PacketSize < 100)
        {
            return "Port Scan";
        }

        // Example: Data exfiltration detection
        if (anomaly.PacketSize > 10000 && anomaly.Protocol == "TCP")
        {
            return "Data Exfiltration";
        }

        // Example: Brute force detection
        if (anomaly.DestinationPort == 22 || anomaly.DestinationPort == 3389)
        {
            return "Brute Force";
        }

        return "Unknown Anomaly";
    }

    private string CalculateSeverityLevel(double anomalyScore)
    {
        // Determine severity based on anomaly score
        if (anomalyScore >= 0.8)
        {
            return "High";
        }
        else if (anomalyScore >= 0.5)
        {
            return "Medium";
        }
        else
        {
            return "Low";
        }
    }

    private string GenerateAlertDescription(NetworkTrafficLog anomaly, string alertType)
    {
        // Generate human-readable description based on type and context
        switch (alertType)
        {
            case "DDoS Attack":
                return $"Potential DDoS attack detected from {anomaly.SourceIp} with {anomaly.PacketCount} packets in {anomaly.Duration} seconds";

            case "Port Scan":
                return $"Port scanning activity detected from {anomaly.SourceIp} to port {anomaly.DestinationPort}";

            case "Data Exfiltration":
                return $"Possible data exfiltration detected from {anomaly.SourceIp} to {anomaly.DestinationIp} with large packet size ({anomaly.PacketSize} bytes)";

            case "Brute Force":
                return $"Potential brute force attack on {anomaly.Protocol} port {anomaly.DestinationPort} from {anomaly.SourceIp}";

            default:
                return $"Unknown anomaly detected from {anomaly.SourceIp} to {anomaly.DestinationIp}:{anomaly.DestinationPort}";
        }
    }

    private IEnumerable<IEnumerable<NetworkTrafficLog>> GroupSimilarAnomalies(IEnumerable<NetworkTrafficLog> anomalies)
    {
        // Group anomalies by similar characteristics to reduce alert noise

        // Example grouping strategy: Group by source IP and destination port within a time window
        var timeWindowMinutes = 5;

        var groups = anomalies
            .GroupBy(a => new
            {
                SourceIp = a.SourceIp,
                DestinationPort = a.DestinationPort,
                TimeWindow = (int)(a.Timestamp.Ticks / (TimeSpan.TicksPerMinute * timeWindowMinutes))
            })
            .Select(g => g.AsEnumerable());

        return groups;
    }
    #endregion
}
