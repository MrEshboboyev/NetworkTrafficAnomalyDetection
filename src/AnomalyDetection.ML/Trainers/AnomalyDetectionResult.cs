using AnomalyDetection.ML.Models;

namespace AnomalyDetection.ML.Trainers;

public class AnomalyDetectionResult
{
    public NetworkTrafficData Data { get; set; }
    public float Score { get; set; }
    public bool IsAnomaly { get; set; }
    public NetworkTrafficPrediction Prediction { get; set; }
}
