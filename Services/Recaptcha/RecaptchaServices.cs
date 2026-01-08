using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models.RecaptchaModels;

namespace DotnetSkeletonApp.Services.Recaptcha
{
    public class RecaptchaServices(
        IHttpClientFactory _httpClientFactory,
        ILogger<RecaptchaServices> _logger,
        IConfiguration _configuration
    )
    {
        public async Task<bool> VerifyAsync(string recaptchaResponse)
        {
            var secretKey = _configuration["GoogleReCaptcha:SecretKey"] ?? "";

            var client = _httpClientFactory.CreateClient();

            var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("secret", secretKey),
                new KeyValuePair<string, string>("response", recaptchaResponse)
            });

            var response = await client.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                content
            );

            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Verify Recaptcha Response: {Json}", json);
            _logger.LogInformation("Secret Key: {secretKey}", secretKey);
            _logger.LogInformation("Recaptcha Response: {recaptchaResponse}", recaptchaResponse);


            var result = JsonSerializer.Deserialize<RecaptchaVerificationModel>(json);

            return result?.Success ?? false;
        }
    }
}