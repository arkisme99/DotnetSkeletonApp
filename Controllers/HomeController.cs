using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DotnetSkeletonApp.Models;

namespace DotnetSkeletonApp.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    // private readonly ILogger<HomeController> _logger = logger;

    public IActionResult Index()
    {
        logger.LogInformation("Home Index");
        return View();
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
