using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotnetSkeletonApp.Services;
using DotnetSkeletonApp.Services.Recaptcha;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSkeletonApp.Controllers
{
    public class AuthController(IConfiguration _config, RecaptchaServices _recaptchaService) : BaseController
    {

        public IActionResult Login()
        {
            var siteKey = _config["GoogleReCaptcha:SiteKey"];
            ViewBag.SiteKey = siteKey;
            return View();
        }

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
                TempData["Notify.Message"] = $"Verifikasi captcha gagal.";

                return RedirectToAction("Login");
            }

            if (await _service.LoginAsync(email, password))
                return RedirectToAction("Index", "Home");

            // ViewBag.Error = "Invalid login attempt.";
            TempData["Notify.Type"] = "error";
            TempData["Notify.Message"] = "Email or Password Is Wrong";

            return RedirectToAction("Login");
        }

        /* [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            TempData["TypeMessage"] = "warning";
            TempData["ValueMessage"] = "Logout Sukses, Terimakasih!";

            return RedirectToAction("Login");
        } */

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }

    }
}