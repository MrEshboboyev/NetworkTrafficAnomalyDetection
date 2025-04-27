using AnomalyDetection.Application.Services;
using AnomalyDetection.Application.ViewModels;
using AnomalyDetection.Application.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace AnomalyDetection.Application.Controllers;

public class HomeController : Controller
{
    private readonly AlertService _alertService;

    public HomeController(AlertService alertService)
    {
        _alertService = alertService;
    }

    public async Task<IActionResult> Index()
    {
        var recentAlerts = await _alertService.GetRecentAlertsAsync(5);

        var viewModel = new DashboardViewModel
        {
            RecentAlerts = recentAlerts
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}