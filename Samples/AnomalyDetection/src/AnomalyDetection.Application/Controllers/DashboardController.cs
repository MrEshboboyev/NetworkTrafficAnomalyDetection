using AnomalyDetection.Application.Services;
using AnomalyDetection.Application.ViewModels;
using AnomalyDetection.Application.Web.ViewModels;
using AnomalyDetection.Domain.Interfaces;
using AnomalyDetection.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalyDetection.Application.Controllers;

public class DashboardController : Controller
{
    private readonly IRepository<NetworkTrafficLog> _repository;
    private readonly AlertService _alertService;

    public DashboardController(
        IRepository<NetworkTrafficLog> repository,
        AlertService alertService)
    {
        _repository = repository;
        _alertService = alertService;
    }

    public async Task<IActionResult> Index()
    {
        // Get all traffic logs from the last 24 hours
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var recentLogs = await _repository.FindAsync(log => log.Timestamp >= yesterday);

        var anomalies = recentLogs.Where(log => log.IsAnomaly).ToList();

        var viewModel = new DashboardViewModel
        {
            RecentAlerts = anomalies,
            TotalTrafficLogs = recentLogs.Count(),
            TotalAnomalies = anomalies.Count,
            AnomalyPercentage = recentLogs.Any() ? (double)anomalies.Count / recentLogs.Count() * 100 : 0
        };

        return View(viewModel);
    }

    public async Task<IActionResult> TrafficSummary()
    {
        // Get data for the last 7 days for the chart
        var startDate = DateTime.UtcNow.AddDays(-7);
        var logs = await _repository.FindAsync(log => log.Timestamp >= startDate);

        // Group logs by day
        var dailyStats = logs
            .GroupBy(log => log.Timestamp.Date)
            .Select(group => new DailyTrafficSummary
            {
                Date = group.Key,
                TotalTraffic = group.Count(),
                AnomalyCount = group.Count(log => log.IsAnomaly)
            })
            .OrderBy(x => x.Date)
            .ToList();

        var viewModel = new TrafficSummaryViewModel
        {
            DailyStats = dailyStats
        };

        return View(viewModel);
    }
}