using AnomalyDetection.ML.Models;
using Microsoft.ML;

namespace AnomalyDetection.ML.Trainers;

public class AnomalyDetectionTrainer
{
    private readonly MLContext _mlContext;

    public AnomalyDetectionTrainer()
    {
        _mlContext = new MLContext(seed: 42);
    }

    public ITransformer TrainPcaModel(
        IEnumerable<NetworkTrafficData> trainingData,
        string modelPath,
        int rank = 3,
        int oversampling = 20,
        bool ensureZeroMean = true)
    {
        var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

        var pipeline = _mlContext.Transforms
            .Concatenate("Features",
                nameof(NetworkTrafficData.SourcePort),
                nameof(NetworkTrafficData.DestinationPort),
                nameof(NetworkTrafficData.PacketSize),
                nameof(NetworkTrafficData.Duration),
                nameof(NetworkTrafficData.PacketCount),
                nameof(NetworkTrafficData.IsTcp),
                nameof(NetworkTrafficData.IsUdp),
                nameof(NetworkTrafficData.IsIcmp))
            .Append(_mlContext.Transforms.NormalizeMinMax("NormalizedFeatures", "Features"))
            .Append(_mlContext.AnomalyDetection.Trainers.RandomizedPca(
                featureColumnName: "NormalizedFeatures",
                rank: rank,
                oversampling: oversampling,
                ensureZeroMean: ensureZeroMean,
                seed: 42));

        var model = pipeline.Fit(dataView);
        _mlContext.Model.Save(model, dataView.Schema, modelPath);
        return model;
    }

    public ITransformer TrainIsolationForestModel(
        IEnumerable<NetworkTrafficData> trainingData,
        string modelPath,
        int numberOfTrees = 100,
        double contamination = 0.1)
    {
        var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

        var pipeline = _mlContext.Transforms
            .Concatenate("Features",
                nameof(NetworkTrafficData.SourcePort),
                nameof(NetworkTrafficData.DestinationPort),
                nameof(NetworkTrafficData.PacketSize),
                nameof(NetworkTrafficData.Duration),
                nameof(NetworkTrafficData.PacketCount),
                nameof(NetworkTrafficData.IsTcp),
                nameof(NetworkTrafficData.IsUdp),
                nameof(NetworkTrafficData.IsIcmp))
            .Append(_mlContext.Transforms.NormalizeMinMax("NormalizedFeatures", "Features"))
            .Append(new IsolationForestTrainer(_mlContext,
                featureColumnName: "NormalizedFeatures",
                numberOfTrees: numberOfTrees,
                contamination: contamination));

        var model = pipeline.Fit(dataView);
        _mlContext.Model.Save(model, dataView.Schema, modelPath);
        return model;
    }

    public ITransformer LoadModel(string modelPath)
    {
        return _mlContext.Model.Load(modelPath, out _);
    }

    public IEnumerable<AnomalyDetectionResult> DetectAnomalies(
        ITransformer model,
        IEnumerable<NetworkTrafficData> testData,
        float threshold = 0.5f)
    {
        var predictionEngine = _mlContext.Model
            .CreatePredictionEngine<NetworkTrafficData, NetworkTrafficPrediction>(model);

        return testData.Select(data =>
        {
            var prediction = predictionEngine.Predict(data);
            return new AnomalyDetectionResult
            {
                Data = data,
                Score = prediction.Score,
                IsAnomaly = prediction.Score > threshold,
                Prediction = prediction
            };
        });
    }

    public ModelMetrics EvaluateModel(
        ITransformer model,
        IEnumerable<NetworkTrafficData> testData,
        IEnumerable<bool> groundTruth)
    {
        var predictions = DetectAnomalies(model, testData)
            .Zip(groundTruth, (pred, truth) => (pred.IsAnomaly, truth))
            .ToList();

        int truePositives = predictions.Count(x => x.IsAnomaly && x.truth);
        int falsePositives = predictions.Count(x => x.IsAnomaly && !x.truth);
        int trueNegatives = predictions.Count(x => !x.IsAnomaly && !x.truth);
        int falseNegatives = predictions.Count(x => !x.IsAnomaly && x.truth);

        double precision = truePositives / (double)(truePositives + falsePositives);
        double recall = truePositives / (double)(truePositives + falseNegatives);
        double accuracy = (truePositives + trueNegatives) / (double)predictions.Count;
        double f1Score = 2 * (precision * recall) / (precision + recall);

        return new ModelMetrics
        {
            Accuracy = accuracy,
            Precision = precision,
            Recall = recall,
            F1Score = f1Score,
            TruePositives = truePositives,
            FalsePositives = falsePositives,
            TrueNegatives = trueNegatives,
            FalseNegatives = falseNegatives
        };
    }
}
