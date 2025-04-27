namespace AnomalyDetection.ML.Models;

public class AnomalyDetectionResult
{
    public NetworkTrafficData Data { get; set; }
    public float Score { get; set; }
    public bool IsAnomaly { get; set; }
    public NetworkTrafficPrediction Prediction { get; set; }
}
