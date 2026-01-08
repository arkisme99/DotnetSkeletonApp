using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotnetSkeletonApp.Helpers.Authorization;
using DotnetSkeletonApp.Services;
using DotnetSkeletonApp.Services.Recaptcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DotnetSkeletonApp.Controllers
{
    public class AuthController(
        IConfiguration _config,
        RecaptchaServices _recaptchaService,
        AuthService _authService,
        IStringLocalizer<SharedResource> _localizer
        ) : BaseController
    {
        [ServiceFilter(typeof(RedirectIfAuthenticated))]
        public IActionResult Login()
        {
            var siteKey = _config["GoogleReCaptcha:SiteKey"];
            ViewBag.SiteKey = siteKey;
            return View();
        }

        [ServiceFilter(typeof(RedirectIfAuthenticated))]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Ambil response dari form
            var recaptchaResponse = Request.Form["g-recaptcha-response"];

            var verifCaptcha = await _recaptchaService.VerifyAsync(recaptchaResponse!);

            if (!verifCaptcha)
            {
                // ModelState.AddModelError(string.Empty, "Verifikasi reCAPTCHA gagal. Silakan coba lagi.");
                TempData["Notify.Type"] = "error";
                TempData["Notify.Message"] = _localizer["PesanCaptchaGagal"].Value;

                return RedirectToAction("Login");
            }

            if (await _authService.LoginAsync(email, password))
                return RedirectToAction("Index", "Home");

            // ViewBag.Error = "Invalid login attempt.";
            TempData["Notify.Type"] = "error";
            TempData["Notify.Message"] = _localizer["PesanLoginGagal"].Value;

            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            TempData["Notify.Type"] = "warning";
            TempData["Notify.Message"] = "Logout Sukses, Terimakasih!";

            return RedirectToAction("Login");
        }

        /* [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        } */

    }
}