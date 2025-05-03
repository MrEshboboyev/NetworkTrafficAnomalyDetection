using AnomalyDetection.Domain.Interfaces;
using AnomalyDetection.Domain.Models;

namespace AnomalyDetection.Infrastructure.ML;

public class AnomalyDetector : ITrafficAnalyzer
{
    private readonly FeatureEngineering _featureEngineering;

    // Precomputed means and standard deviations for features
    private readonly float[] _featureMeans = { 5000, 3000, 120, 12, 0.3f, 0.4f, 0.3f, 0.5f, 0.5f };
    private readonly float[] _featureStdDevs = { 10000, 8000, 300, 7, 0.45f, 0.2f, 0.2f, 0.3f, 0.3f };

    // Isolation Forest parameters
    //private readonly int _sampleSize = 100;
    private readonly int _treeCount = 100;
    private readonly Random _random = new(42);

    public AnomalyDetector()
    {
        _featureEngineering = new FeatureEngineering();
    }

    public async Task<IEnumerable<NetworkTrafficLog>> AnalyzeTrafficAsync(IEnumerable<NetworkTrafficLog> logs)
    {
        List<NetworkTrafficLog> analyzedLogs = [];

        foreach (var log in logs)
        {
            double score = await CalculateAnomalyScoreAsync(log);
            log.AnomalyScore = score;
            log.IsAnomaly = score > 0.8;
            analyzedLogs.Add(log);
        }

        return analyzedLogs;
    }

    public async Task<double> CalculateAnomalyScoreAsync(NetworkTrafficLog log)
    {
        // Extract and normalize features
        float[] features = _featureEngineering.ExtractFeatures(log);
        float[] normalizedFeatures = _featureEngineering.NormalizeFeatures(features, _featureMeans, _featureStdDevs);

        // Simulate ML model prediction using Isolation Forest algorithm
        double score = await Task.Run(() => IsolationForestScore(normalizedFeatures));

        return score;
    }

    public async Task<bool> IsAnomalousAsync(NetworkTrafficLog log, double threshold = 0.8)
    {
        double score = await CalculateAnomalyScoreAsync(log);
        return score > threshold;
    }

    // Simplified implementation of Isolation Forest anomaly detection
    private double IsolationForestScore(float[] features)
    {
        // In a real implementation, this would use a proper ML library
        // This is a simplified version for demonstration

        double avgPathLength = 0;

        // Calculate average path length across multiple trees
        for (int i = 0; i < _treeCount; i++)
        {
            avgPathLength += PathLengthFromRandomTree(features);
        }

        avgPathLength /= _treeCount;

        // Normalize the score between 0 and 1
        double score = Math.Pow(2, -avgPathLength / AveragePathLength(features.Length));

        return score;
    }

    private double PathLengthFromRandomTree(float[] features)
    {
        // Simplified simulation of isolation forest algorithm
        // In a real implementation, this would build a proper isolation tree

        double pathLength = 0;
        int remainingFeatures = features.Length;

        while (remainingFeatures > 0 && pathLength < 10)
        {
            // Choose a random feature
            int featureIndex = _random.Next(0, features.Length);

            // The more unusual the feature value, the shorter the path
            double unusualness = Math.Abs(features[featureIndex]);

            // Increase path length
            pathLength += 1 + (1 / (1 + unusualness));

            // Reduce remaining features
            remainingFeatures--;
        }

        return pathLength;
    }

    private double AveragePathLength(int n)
    {
        // Formula for average path length in isolation forest
        if (n <= 1) return 0;
        return 2 * (Math.Log(n - 1) + 0.5772156649) - (2 * (n - 1) / n);
    }
}
