using AnomalyDetection.Application.Services;
using AnomalyDetection.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AnomalyDetection.Web.Controllers;

public class HomeController(AlertService alertService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var recentAlerts = await alertService.GetRecentAlertsAsync(5);

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
