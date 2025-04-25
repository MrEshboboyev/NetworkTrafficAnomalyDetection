using AnomalyDetection.Core.Entities;
using AnomalyDetection.ML.Models;

namespace AnomalyDetection.Web.Services.FeatureEngineering;

public class FeatureExtractor
{
    public IEnumerable<NetworkTrafficData> ExtractFeatures(IEnumerable<NetworkTrafficLog> logs)
    {
        var features = new List<NetworkTrafficData>();

        foreach (var log in logs)
        {
            var trafficData = new NetworkTrafficData
            {
                SourcePort = log.SourcePort,
                DestinationPort = log.DestinationPort,
                PacketSize = log.PacketSize,
                Duration = (float)log.Duration,
                PacketCount = log.PacketCount,

                // Convert protocols to one-hot encoding
                IsTcp = log.Protocol.Equals("TCP", StringComparison.OrdinalIgnoreCase) ? 1 : 0,
                IsUdp = log.Protocol.Equals("UDP", StringComparison.OrdinalIgnoreCase) ? 1 : 0,
                IsIcmp = log.Protocol.Equals("ICMP", StringComparison.OrdinalIgnoreCase) ? 1 : 0
            };

            features.Add(trafficData);
        }

        return features;
    }

    public IEnumerable<NetworkTrafficData> ComputeAdditionalFeatures(IEnumerable<NetworkTrafficLog> logs)
    {
        var features = ExtractFeatures(logs).ToList();

        // Group logs by source IP for behavior analysis
        var sourceIpGroups = logs.GroupBy(l => l.SourceIp);

        foreach (var group in sourceIpGroups)
        {
            string sourceIp = group.Key;
            var sourceIpLogs = group.ToList();

            // Calculate source IP metrics
            int uniqueDestinations = sourceIpLogs.Select(l => l.DestinationIp).Distinct().Count();
            int uniquePorts = sourceIpLogs.Select(l => l.DestinationPort).Distinct().Count();
            double avgPacketSize = sourceIpLogs.Average(l => l.PacketSize);
            int totalPackets = sourceIpLogs.Sum(l => l.PacketCount);

            // Time-based metrics
            DateTime minTime = sourceIpLogs.Min(l => l.Timestamp);
            DateTime maxTime = sourceIpLogs.Max(l => l.Timestamp);
            TimeSpan timeSpan = maxTime - minTime;
            double connectedTimeMinutes = timeSpan.TotalMinutes;

            // Connect these additional metrics with the feature data
            foreach (var log in sourceIpLogs)
            {
                var index = logs.ToList().IndexOf(log);
                if (index >= 0 && index < features.Count)
                {
                    // Additional features could be added here
                    // For example, connection rate, protocol distribution, etc.

                    // Note: In a real implementation, you would need to add these fields
                    // to the NetworkTrafficData class
                }
            }
        }

        return features;
    }

    public IEnumerable<NetworkTrafficData> NormalizeFeatures(IEnumerable<NetworkTrafficLog> features, out NormalizationParams normalizationParams)
    {
        // First, convert NetworkTrafficLog to NetworkTrafficData
        var networkTrafficData = features.Select(log => new NetworkTrafficData
        {
            SourcePort = log.SourcePort,
            DestinationPort = log.DestinationPort,
            PacketSize = log.PacketSize,
            //Duration = log.Duration,
            PacketCount = log.PacketCount,
            //IsTcp = log.IsTcp,
            //IsUdp = log.IsUdp,
            //IsIcmp = log.IsIcmp
        }).ToList();

        // Calculate the normalization parameters (min, max) for each feature
        normalizationParams = new NormalizationParams
        {
            MinSourcePort = networkTrafficData.Min(f => f.SourcePort),
            MaxSourcePort = networkTrafficData.Max(f => f.SourcePort),
            MinDestPort = networkTrafficData.Min(f => f.DestinationPort),
            MaxDestPort = networkTrafficData.Max(f => f.DestinationPort),
            MinPacketSize = networkTrafficData.Min(f => f.PacketSize),
            MaxPacketSize = networkTrafficData.Max(f => f.PacketSize),
            MinDuration = networkTrafficData.Min(f => f.Duration),
            MaxDuration = networkTrafficData.Max(f => f.Duration),
            MinPacketCount = networkTrafficData.Min(f => f.PacketCount),
            MaxPacketCount = networkTrafficData.Max(f => f.PacketCount)
        };

        var normalizedFeatures = new List<NetworkTrafficData>();

        // Iterate through each feature and apply normalization
        foreach (var feature in networkTrafficData)
        {
            var normalized = new NetworkTrafficData
            {
                SourcePort = NormalizeValue(feature.SourcePort, normalizationParams.MinSourcePort, normalizationParams.MaxSourcePort),
                DestinationPort = NormalizeValue(feature.DestinationPort, normalizationParams.MinDestPort, normalizationParams.MaxDestPort),
                PacketSize = NormalizeValue(feature.PacketSize, normalizationParams.MinPacketSize, normalizationParams.MaxPacketSize),
                Duration = NormalizeValue(feature.Duration, normalizationParams.MinDuration, normalizationParams.MaxDuration),
                PacketCount = NormalizeValue(feature.PacketCount, normalizationParams.MinPacketCount, normalizationParams.MaxPacketCount),

                // One-hot encoded features are already normalized, no need to modify them
                IsTcp = feature.IsTcp,
                IsUdp = feature.IsUdp,
                IsIcmp = feature.IsIcmp
            };

            normalizedFeatures.Add(normalized);
        }

        return normalizedFeatures;
    }

    private float NormalizeValue(float value, float minValue, float maxValue)
    {
        // Min-Max normalization to [0, 1]
        return (value - minValue) / (maxValue - minValue);
    }
}
