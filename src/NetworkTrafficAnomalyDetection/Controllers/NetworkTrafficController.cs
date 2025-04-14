using Microsoft.AspNetCore.Mvc;
using NetworkTrafficAnomalyDetection.Data;
using NetworkTrafficAnomalyDetection.MLModel;
using NetworkTrafficAnomalyDetection.Models;

namespace NetworkTrafficAnomalyDetection.Controllers;

public class NetworkTrafficController(NetworkTrafficContext context) : Controller
{
    private readonly AnomalyDetectionModel _anomalyDetectionModel = new AnomalyDetectionModel();

    // GET: NetworkTraffic
    public IActionResult Index()
    {
        var trafficData = context.NetworkTraffics.ToList();
        return View(trafficData);
    }

    // POST: NetworkTraffic/Analyze
    [HttpPost]
    public IActionResult Analyze(NetworkTraffic traffic)
    {
        // Prepare the feature array (for example: IP address length, port, packet length)
        var features = new float[] { traffic.IpAddress.Length, traffic.Port, traffic.PacketLength };

        // Example test data for training/testing purposes
        var testTrafficData = new List<float[]>
        {
            new float[] { 192, 80, 1500 }, // Example 1: Typical network traffic
            new float[] { 200, 8080, 3000 }, // Example 2: Possible anomaly
            new float[] { 175, 443, 1200 }, // Example 3: Normal traffic
            new float[] { 130, 60000, 3000 } // Example 4: Possible anomaly
        };

        // Train the model with test data
        foreach (var data in testTrafficData)
        {
            _anomalyDetectionModel.TrainModel(new[] { data }); // Assuming TrainModel method is updated to handle this
        }


        // Use the ML model to predict if the network traffic is an anomaly
        var result = _anomalyDetectionModel.PredictAnomaly(features);

        // Set the anomaly label in the traffic object
        traffic.IsAnomaly = result; // Anomaly is detected if label is 1

        // Save the result to the database
        context.NetworkTraffics.Add(traffic);
        context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
