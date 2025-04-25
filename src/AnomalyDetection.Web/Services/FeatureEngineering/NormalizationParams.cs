namespace AnomalyDetection.Web.Services.FeatureEngineering;

public class NormalizationParams
{
    public float MinSourcePort { get; set; }
    public float MaxSourcePort { get; set; }
    public float MinDestPort { get; set; }
    public float MaxDestPort { get; set; }
    public float MinPacketSize { get; set; }
    public float MaxPacketSize { get; set; }
    public float MinDuration { get; set; }
    public float MaxDuration { get; set; }
    public float MinPacketCount { get; set; }
    public float MaxPacketCount { get; set; }
}
