namespace AnomalyDetection.Core.Entities;

public class MLModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ModelType { get; set; }  // Algorithm type
    public DateTime CreatedAt { get; set; }
    public DateTime? LastTrainedAt { get; set; }
    public string FilePath { get; set; }   // Path to saved model file
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score { get; set; }
}