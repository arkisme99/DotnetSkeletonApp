using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
            await _hub.Clients.Group(userId).SendAsync(methodName, new Notification
            {
                Message = notif.Message,
                FileUrl = notif.FileUrl,
                // Time = DateTime.Now.ToString("HH:mm")
                CreatedAt = notif.CreatedAt
            });
        }

        public async Task<List<Notification>> GetAlLDataAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid id, string userId)
        {
            var notif = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notif != null)
            {
                notif.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAsReadAllAsync(string userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (unreadNotifications.Count != 0)
            {
                // 2. Loop semua data dan ubah statusnya
                foreach (var notif in unreadNotifications)
                {
                    notif.IsRead = true;
                }

                // 3. Simpan perubahan sekaligus (Hanya 1 kali panggil database)
                await _context.SaveChangesAsync();
            }
        }
    }
}