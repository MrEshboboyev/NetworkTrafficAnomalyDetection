using Microsoft.ML.Data;

namespace AnomalyDetection.ML.Models;

public class NetworkTrafficData
{
    [LoadColumn(0)]
    public float SourcePort { get; set; }

    [LoadColumn(1)]
    public float DestinationPort { get; set; }

    [LoadColumn(2)]
    public float PacketSize { get; set; }

    [LoadColumn(3)]
    public float Duration { get; set; }

    [LoadColumn(4)]
    public float PacketCount { get; set; }

    [LoadColumn(5)]
    public float IsTcp { get; set; }

    [LoadColumn(6)]
    public float IsUdp { get; set; }

    [LoadColumn(7)]
    public float IsIcmp { get; set; }

    // Helper property to count features
    public static int FeatureCount => typeof(NetworkTrafficData)
        .GetProperties()
        .Count(p => p.PropertyType == typeof(float));
}
