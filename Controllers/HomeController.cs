using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;

namespace DotnetSkeletonApp.Controllers;

[Authorize]
public class HomeController(ILogger<HomeController> _logger) : BaseController
{
    public IActionResult Index()
    {

        SetBreadcrumbs(
            new BreadcrumbsViewModel("Home", false, "Home", "Index"),
            new BreadcrumbsViewModel("Privacy", false, "Home", "Privacy"),
            new BreadcrumbsViewModel("About", true, "About", "Index")
        );

        _logger.LogInformation("Masuk Ke Home Index");
        return View();
    }

    public IActionResult Privacy()
    {
        SetBreadcrumbs(
            new BreadcrumbsViewModel("Home", false, "Home", "Index"),
            new BreadcrumbsViewModel("Privacy", true, "Home", "Privacy")
        );
        return View();
    }

    [HttpGet]
    public IActionResult SetLanguage(string culture, string returnUrl = "/")
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
    }

    /* [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    } */
}
