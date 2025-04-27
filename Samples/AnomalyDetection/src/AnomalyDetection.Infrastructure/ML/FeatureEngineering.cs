using AnomalyDetection.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalyDetection.Infrastructure.ML;

public class FeatureEngineering
{
    // Extracts features from raw network traffic data
    public float[] ExtractFeatures(NetworkTrafficLog log)
    {
        return new float[]
        {
                log.BytesSent,
                log.BytesReceived,
                log.ConnectionDuration,
                CalculateTimeOfDayFeature(log.Timestamp),
                IsWeekend(log.Timestamp) ? 1 : 0,
                CalculatePortRiskLevel(log.Port),
                CalculateProtocolRiskLevel(log.Protocol),
                CalculateIpReputationScore(log.SourceIp),
                CalculateIpReputationScore(log.DestinationIp)
        };
    }

    // Normalize features to have consistent scale
    public float[] NormalizeFeatures(float[] features, float[] means, float[] stdDevs)
    {
        float[] normalizedFeatures = new float[features.Length];

        for (int i = 0; i < features.Length; i++)
        {
            normalizedFeatures[i] = (features[i] - means[i]) / stdDevs[i];
        }

        return normalizedFeatures;
    }

    // Calculate time of day as a continuous feature (0-24 hours)
    private float CalculateTimeOfDayFeature(DateTime timestamp)
    {
        return (float)(timestamp.Hour + timestamp.Minute / 60.0);
    }

    // Check if the timestamp is on a weekend
    private bool IsWeekend(DateTime timestamp)
    {
        return timestamp.DayOfWeek == DayOfWeek.Saturday || timestamp.DayOfWeek == DayOfWeek.Sunday;
    }

    // Assign risk level based on port number
    private float CalculatePortRiskLevel(int port)
    {
        // Common ports have lower risk, unusual ports have higher risk
        int[] commonPorts = { 80, 443, 22, 25, 53, 110, 143, 993, 995 };

        if (commonPorts.Contains(port))
            return 0.2f;
        else if (port < 1024)
            return 0.5f;
        else
            return 0.8f;
    }

    // Assign risk level based on protocol
    private float CalculateProtocolRiskLevel(string protocol)
    {
        switch (protocol.ToUpper())
        {
            case "HTTPS":
                return 0.1f;
            case "HTTP":
                return 0.3f;
            case "SSH":
                return 0.2f;
            case "FTP":
                return 0.6f;
            case "TELNET":
                return 0.9f;
            default:
                return 0.7f;
        }
    }

    // Simulated IP reputation score (in a real system, this would query a reputation database)
    private float CalculateIpReputationScore(string ip)
    {
        // This is a placeholder - in a real system you would check against an IP reputation database
        // For now, just return a random value between 0 and 1
        Random random = new Random(ip.GetHashCode());
        return (float)random.NextDouble();
    }
}