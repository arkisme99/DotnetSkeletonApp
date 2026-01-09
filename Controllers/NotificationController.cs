using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSkeletonApp.Controllers
{
    [Authorize]
    public class NotificationController(
        NotificationService _service
    ) : Controller
    {
        [HttpGet("get-all-notifications")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await _service.GetAlLDataAsync(userId!);
            return Ok(new { count = data.Count, items = data });
        }

        [HttpPost("read-notification/{idNotification}")]
        public async Task<IActionResult> ReadNotification(Guid idNotification)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _service.MarkAsReadAsync(idNotification, userId!);
                return Ok(new { success = true, message = "Notifikasi telah dibaca" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("read-all-notifications")]
        public async Task<IActionResult> ReadAllNotification()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _service.MarkAsReadAllAsync(userId!);
                return Ok(new { success = true, message = "Notifikasi telah dibaca" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}