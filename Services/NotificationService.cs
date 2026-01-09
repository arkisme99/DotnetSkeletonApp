using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace DotnetSkeletonApp.Services
{
    public class NotificationService(
        ApplicationDbContext _context,
        IHubContext<JobNotificationHub> _hub
    )
    {
        public async Task AddNotificationAsync(string userId, string message, string? fileUrl = null, string methodName = "ReceiveNotification")
        {
            var notif = new Notification
            {
                UserId = userId,
                Message = message,
                FileUrl = fileUrl,
                MethodName = methodName
            };

            _context.Notifications.Add(notif);
            await _context.SaveChangesAsync();

            // Kirim real-time hanya ke user yang bersangkutan
            await _hub.Clients.Group(userId).SendAsync(methodName, new
            {
                message = notif.Message,
                fileUrl = notif.FileUrl,
                // time = DateTime.Now.ToString("HH:mm")
                time = notif.CreatedAt
            });
        }
    }
}