namespace AnomalyDetection.Core.Entities;

public class MLModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ModelType { get; set; }  // "PCA", "IsolationForest", etc.
    public DateTime CreatedAt { get; set; }
    public DateTime LastTrainedAt { get; set; }
    public DateTime? LastEvaluatedAt { get; set; }
    public string FilePath { get; set; }

    // Training parameters
    public string TrainingParameters { get; set; }
    public int? NumberOfFeatures { get; set; }
    public int? TrainingDataSize { get; set; }

    // Evaluation metrics
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score { get; set; }

    // Threshold information
    public float? DecisionThreshold { get; set; }

    // Versioning
    public string Version { get; set; } = "1.0";
    public bool IsProduction { get; set; } = false;

    // Additional metadata
    public Dictionary<string, string> Metadata { get; set; } = [];
}
