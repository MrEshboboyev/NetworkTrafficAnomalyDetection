using Microsoft.ML;
using Microsoft.ML.Data;
using System.Linq;

namespace NetworkTrafficAnomalyDetection.MLModel
{
    public class AnomalyData
    {
        // Use Vector<float> instead of float[] for the Features column
        [VectorType(3)]
        public float[] Features { get; set; }
    }

    public class AnomalyPrediction
    {
        public float PredictedLabel { get; set; }
    }

    public class AnomalyDetectionModel
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public AnomalyDetectionModel()
        {
            _mlContext = new MLContext();
        }

        // Train the anomaly detection model
        public void TrainModel(float[][] data)
        {
            // Convert data into AnomalyData format
            var trainingData = data.Select(d => new AnomalyData { Features = d }).ToArray();

            // Load the data into an IDataView
            var trainData = _mlContext.Data.LoadFromEnumerable(trainingData);

            // Create a pipeline: feature concatenation + RandomizedPCA trainer
            var pipeline = _mlContext.Transforms.Concatenate("Features", nameof(AnomalyData.Features))
                .Append(_mlContext.AnomalyDetection.Trainers.RandomizedPca(featureColumnName: "Features", rank: 20)); // Rank should be <= number of features

            // Fit the model
            _model = pipeline.Fit(trainData);
        }

        // Predict if the traffic is an anomaly
        public bool PredictAnomaly(float[] data)
        {
            // Create a prediction engine from the trained model
            var predictionFunction = _mlContext.Model.CreatePredictionEngine<AnomalyData, AnomalyPrediction>(_model);

            // Predict the label for the given data
            var prediction = predictionFunction.Predict(new AnomalyData { Features = data });

            // Return true if it's an anomaly
            return prediction.PredictedLabel == 1;
        }
    }
}
