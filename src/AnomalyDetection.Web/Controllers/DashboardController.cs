using AnomalyDetection.Core.Entities;
using AnomalyDetection.Web.Models;
using AnomalyDetection.Web.Services.DataIngestion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnomalyDetection.Web.Controllers;

//[Authorize]
public class DashboardController(LogParserFactory logParserFactory) : Controller
{
    public IActionResult Index()
    {
        var dashboardViewModel = new DashboardViewModel
        {
            RecentAlerts = GetRecentAlerts(),
            TrafficSummary = GetTrafficSummary(),
            AnomalyStatistics = GetAnomalyStatistics()
        };

        return View(dashboardViewModel);
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(UploadLogViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            using var stream = model.LogFile.OpenReadStream();
            var parser = logParserFactory.GetParser(model.LogFile.FileName);
            var logs = await parser.ParseAsync(stream);

            // TODO: Process logs, run anomaly detection, and save results

            TempData["SuccessMessage"] = $"Successfully processed {logs.Count()} log entries.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error processing file: {ex.Message}");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Anomalies()
    {
        var anomalies = GetAnomalies(); // Get from repository
        return View(anomalies);
    }

    [HttpGet]
    public IActionResult AnomalyDetails(int id)
    {
        var anomaly = GetAnomalyById(id); // Get from repository

        if (anomaly == null)
        {
            return NotFound();
        }

        return View(anomaly);
    }

    #region Helper Methods
    private List<AnomalyAlert> GetRecentAlerts()
    {
        // TODO: Get from repository
        return
            [
                new() {
                    Id = 1,
                    Timestamp = DateTime.Now.AddHours(-1),
                    AlertType = "DDoS Attack",
                    Description = "Potential DDoS attack detected from multiple IPs",
                    SeverityLevel = "High",
                    ConfidenceScore = 0.92
                },
                new() {
                    Id = 2,
                    Timestamp = DateTime.Now.AddHours(-3),
                    AlertType = "Port Scan",
                    Description = "Port scanning activity detected from 192.168.1.5",
                    SeverityLevel = "Medium",
                    ConfidenceScore = 0.78
                }
            ];
    }

    private TrafficSummaryViewModel GetTrafficSummary()
    {
        // TODO: Get from repository
        return new TrafficSummaryViewModel
        {
            TotalPackets = 128500,
            AveragePacketSize = 1240,
            TopSourceIps = new Dictionary<string, int>
                {
                    { "192.168.1.5", 5420 },
                    { "10.0.0.15", 3250 },
                    { "172.16.0.8", 2180 }
                },
            TopDestinationPorts = new Dictionary<int, int>
                {
                    { 443, 8200 },
                    { 80, 5300 },
                    { 22, 1580 }
                }
        };
    }

    private AnomalyStatisticsViewModel GetAnomalyStatistics()
    {
        // TODO: Get from repository
        return new AnomalyStatisticsViewModel
        {
            TotalAnomalies = 37,
            HighSeverity = 8,
            MediumSeverity = 14,
            LowSeverity = 15,
            AnomalyTypes = new Dictionary<string, int>
                {
                    { "DDoS", 12 },
                    { "Port Scan", 9 },
                    { "Data Exfiltration", 5 },
                    { "Brute Force", 7 },
                    { "Other", 4 }
                }
        };
    }

    private List<NetworkTrafficLog> GetAnomalies()
    {
        // TODO: Get from repository
        return
            [
                new() {
                    Id = 1,
                    Timestamp = DateTime.Now.AddHours(-2),
                    SourceIp = "192.168.1.5",
                    DestinationIp = "10.0.0.25",
                    SourcePort = 45123,
                    DestinationPort = 80,
                    Protocol = "TCP",
                    PacketSize = 1280,
                    Duration = 0.05,
                    IsAnomaly = true,
                    AnomalyScore = 0.95,
                    AnomalyType = "DDoS"
                }
            ];
    }

    private NetworkTrafficLog GetAnomalyById(int id)
    {
        // TODO: Get from repository
        return GetAnomalies().FirstOrDefault(a => a.Id == id);
    }
    #endregion
}
