using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Hangfire;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.SignalR;
using DotnetSkeletonApp.Notifications;
using System.Security.Claims;

namespace DotnetSkeletonApp.Controllers;

[Authorize]
public class HomeController(
    ILogger<HomeController> _logger,
    IBackgroundJobClient _jobs
    ) : BaseController
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

    [HttpGet]
    public IActionResult TestEmail()
    {

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Simpan JobId-nya
        var jobId = _jobs.Enqueue<EmailService>(svc => svc.SendEmailAsync(
            0, "penerima@email.com", "Tes Kirim Lagi", "Berhasil kirim email"
        ));

        // "Titipkan" userId ke dalam storage Hangfire berdasarkan jobId tersebut
        using (var connection = JobStorage.Current.GetConnection())
        {
            connection.SetJobParameter(jobId, "CreatorUserId", currentUserId);
            connection.SetJobParameter(jobId, "JobNameNew", "Tes Kirim Email");
        }

        TempData["Notify.Type"] = "warning";
        TempData["Notify.Message"] = "Informasi pengiriman akan ada di notifikasi";

        return RedirectToAction("Index");
    }

    /* [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    } */
}
