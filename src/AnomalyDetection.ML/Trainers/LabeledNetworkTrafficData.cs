using AnomalyDetection.ML.Models;

namespace AnomalyDetection.ML.Trainers;

// Helper class for evaluation
public class LabeledNetworkTrafficData
{
    public NetworkTrafficData Data { get; set; }
    public bool Label { get; set; }
}
