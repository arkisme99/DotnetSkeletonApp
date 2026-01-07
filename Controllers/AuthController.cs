using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSkeletonApp.Controllers
{
    public class AuthController() : Controller
    {

        public IActionResult Login(string? message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                TempData["TypeMessage"] = "error";
                TempData["ValueMessage"] = message;
            }

            // var siteKey = _config["GoogleReCaptcha:SiteKey"];
            // ViewBag.SiteKey = siteKey;
            return View();
        }

        /* [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Ambil response dari form
            var recaptchaResponse = Request.Form["g-recaptcha-response"];
            var secretKey = _config["GoogleReCaptcha:SecretKey"];

            var verifCaptcha = await VerifyRecaptchaAsync(secretKey!, recaptchaResponse!);

            if (!verifCaptcha)
            {
                // ModelState.AddModelError(string.Empty, "Verifikasi reCAPTCHA gagal. Silakan coba lagi.");
                TempData["TypeMessage"] = "error";
                TempData["ValueMessage"] = $"Verifikasi captcha gagal.";
                Console.WriteLine($"sec: {secretKey}");
                Console.WriteLine($"rec: {recaptchaResponse}");

                return RedirectToAction("Login");
            }

            if (await _service.LoginAsync(email, password))
                return RedirectToAction("Index", "Home");

            // ViewBag.Error = "Invalid login attempt.";
            TempData["TypeMessage"] = "error";
            TempData["ValueMessage"] = "Email or Password Is Wrong";

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            TempData["TypeMessage"] = "warning";
            TempData["ValueMessage"] = "Logout Sukses, Terimakasih!";

            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error");
        }

        private async Task<bool> VerifyRecaptchaAsync(string secretKey, string recaptchaResponse)
        {
            var client = _httpClientFactory.CreateClient();

            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("secret", secretKey),
                new KeyValuePair<string, string>("response", recaptchaResponse)
            ]);

            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<RecaptchaVerificationResponse>(json);
            Console.WriteLine(json); // <-- Tambahkan log ini sementara

            return result?.Success ?? false;
        }

        private class RecaptchaVerificationResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("challenge_ts")]
            public string? Challenge_ts { get; set; }

            [JsonPropertyName("hostname")]
            public string? Hostname { get; set; }

            [JsonPropertyName("error-codes")]
            public List<string>? ErrorCodes { get; set; }
        } */
    }
}