using AnomalyDetection.Core.Entities;
using AnomalyDetection.Core.Repositories;
using AnomalyDetection.ML.Models;
using AnomalyDetection.ML.Trainers;
using AnomalyDetection.Web.Models;
using AnomalyDetection.Web.Services.FeatureEngineering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using System.Text.Json;

namespace AnomalyDetection.Web.Controllers;

//[Authorize(Roles = "Admin")]
public class ModelController : Controller
{
    private readonly AnomalyDetectionTrainer _trainer;
    private readonly FeatureExtractor _featureExtractor;
    private readonly IModelRepository _modelRepository;
    private readonly INetworkTrafficRepository _trafficRepository;
    private readonly ILogger<ModelController> _logger;
    private readonly string _modelDirectory;

    public ModelController(
        AnomalyDetectionTrainer trainer,
        FeatureExtractor featureExtractor,
        IModelRepository modelRepository,
        INetworkTrafficRepository trafficRepository,
        IWebHostEnvironment environment,
        ILogger<ModelController> logger)
    {
        _trainer = trainer;
        _featureExtractor = featureExtractor;
        _modelRepository = modelRepository;
        _trafficRepository = trafficRepository;
        _logger = logger;
        _modelDirectory = Path.Combine(environment.ContentRootPath, "Models");

        if (!Directory.Exists(_modelDirectory))
        {
            Directory.CreateDirectory(_modelDirectory);
        }
    }

    public async Task<IActionResult> Index()
    {
        var models = await _modelRepository.GetAllAsync();
        return View(models);
    }

    [HttpGet]
    public IActionResult Train()
    {
        return View(new TrainModelViewModel
        {
            ModelTypes = ["PCA", "IsolationForest"]
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Train(TrainModelViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ModelTypes = ["PCA", "IsolationForest"];
            return View(model);
        }

        try
        {
            var trainingLogs = await _trafficRepository.GetTrainingDataAsync();
            var features = _featureExtractor.ExtractFeatures(trainingLogs).ToList();
            NormalizationParams normalizationParams;
            var normalizedFeatures = _featureExtractor.NormalizeFeatures(trainingLogs, out normalizationParams);

            string modelName = $"{model.Name}_{DateTime.Now:yyyyMMddHHmmss}.zip";
            string modelPath = Path.Combine(_modelDirectory, modelName);

            ITransformer trainedModel;
            string trainingParams;

            if (model.SelectedModelType == "PCA")
            {
                trainedModel = _trainer.TrainPcaModel(
                    normalizedFeatures,
                    modelPath,
                    rank: model.NumberOfComponents ?? 3);
                trainingParams = $"PCA - Components: {model.NumberOfComponents}";
            }
            else if (model.SelectedModelType == "IsolationForest")
            {
                trainedModel = _trainer.TrainIsolationForestModel(
                    normalizedFeatures,
                    modelPath,
                    contamination: model.Contamination ?? 0.1);
                trainingParams = $"IsolationForest - Contamination: {model.Contamination}, Trees: 100";
            }
            else
            {
                throw new NotSupportedException($"Model type {model.SelectedModelType} is not supported.");
            }

            var mlModel = new MLModel
            {
                Name = model.Name,
                Description = model.Description,
                ModelType = model.SelectedModelType,
                CreatedAt = DateTime.UtcNow,
                LastTrainedAt = DateTime.UtcNow,
                FilePath = modelPath,
                TrainingParameters = trainingParams,
                NumberOfFeatures = typeof(NetworkTrafficData).GetProperties()
                    .Count(p => p.PropertyType == typeof(float)),
                TrainingDataSize = normalizedFeatures.Count(),
                DecisionThreshold = 0.5f,
                Metadata = new Dictionary<string, string>
                {
                    {"FeatureExtractorVersion", "1.0"},
                    {"NormalizationMethod", "MinMax"},
                    {"FeatureNames", string.Join(",", new[] {
                        nameof(NetworkTrafficData.SourcePort),
                        nameof(NetworkTrafficData.DestinationPort),
                        nameof(NetworkTrafficData.PacketSize),
                        nameof(NetworkTrafficData.Duration),
                        nameof(NetworkTrafficData.PacketCount),
                        nameof(NetworkTrafficData.IsTcp),
                        nameof(NetworkTrafficData.IsUdp),
                        nameof(NetworkTrafficData.IsIcmp)
                    })}
                }
            };

            await _modelRepository.AddAsync(mlModel);

            _logger.LogInformation("Successfully trained model: {ModelName}", model.Name);
            TempData["SuccessMessage"] = $"Successfully trained model: {model.Name}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training model");
            ModelState.AddModelError(string.Empty, $"Error training model: {ex.Message}");
            model.ModelTypes = ["PCA", "IsolationForest"];
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Evaluate(int id)
    {
        var model = await _modelRepository.GetByIdAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        return View(new EvaluateModelViewModel
        {
            ModelId = id,
            ModelName = model.Name
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Evaluate(EvaluateModelViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var testLogs = await _trafficRepository.GetTestDataAsync();
            var features = _featureExtractor.ExtractFeatures(testLogs).ToList();
            NormalizationParams normalizationParams;
            var normalizedFeatures = _featureExtractor.NormalizeFeatures(testLogs, out normalizationParams);
            var labels = testLogs.Select(x => x.IsAnomaly).ToList();

            var mlModel = await _modelRepository.GetByIdAsync(model.ModelId);
            if (mlModel == null)
            {
                return NotFound();
            }

            var trainedModel = _trainer.LoadModel(mlModel.FilePath);
            var metrics = _trainer.EvaluateModel(trainedModel, normalizedFeatures, labels);

            // Update model metrics
            mlModel.Accuracy = metrics.Accuracy;
            mlModel.Precision = metrics.Precision;
            mlModel.Recall = metrics.Recall;
            mlModel.F1Score = metrics.F1Score;
            mlModel.LastEvaluatedAt = DateTime.UtcNow;
            mlModel.Metadata["EvaluationDate"] = DateTime.UtcNow.ToString("O");
            mlModel.Metadata["TestDataSize"] = normalizedFeatures.Count().ToString();

            await _modelRepository.UpdateAsync(mlModel);

            _logger.LogInformation("Evaluated model {ModelId} with F1 score {F1Score}",
                model.ModelId, metrics.F1Score);
            TempData["EvaluationResults"] = JsonSerializer.Serialize(metrics);
            TempData["SuccessMessage"] = $"Successfully evaluated model. F1 Score: {metrics.F1Score:P2}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating model {ModelId}", model.ModelId);
            ModelState.AddModelError(string.Empty, $"Error evaluating model: {ex.Message}");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var model = await _modelRepository.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            if (System.IO.File.Exists(model.FilePath))
            {
                System.IO.File.Delete(model.FilePath);
            }

            await _modelRepository.DeleteAsync(id);

            _logger.LogInformation("Deleted model {ModelId}", id);
            TempData["SuccessMessage"] = $"Successfully deleted model: {model.Name}";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting model {ModelId}", id);
            TempData["ErrorMessage"] = $"Error deleting model: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Download(int id)
    {
        var model = await _modelRepository.GetByIdAsync(id);
        if (model == null || !System.IO.File.Exists(model.FilePath))
        {
            return NotFound();
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(model.FilePath);
        return File(fileBytes, "application/zip", $"{model.Name}.zip");
    }
}
