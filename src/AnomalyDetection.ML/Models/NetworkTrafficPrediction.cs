using Microsoft.ML.Data;

namespace AnomalyDetection.ML.Models;

// Output prediction class
public class NetworkTrafficPrediction
{
    // For anomaly detection
    [VectorType(1)]
    public float[] PredictedLabel { get; set; }

    // For PCA-based anomaly detection
    public float Score { get; set; }
}
