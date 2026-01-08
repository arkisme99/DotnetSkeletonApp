using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models;

namespace DotnetSkeletonApp.Services
{
    public class ActivityLogService(ApplicationDbContext _dbcontext)
    {
        public async Task LogChangeAsync(
        string? entityName,
        string stringAction,
        string user,
        string? entityId,
        object? changes)
        {
            var log = new ActivityLog
            {
                EntityName = entityName,
                Action = stringAction,
                ChangedBy = user,
                EntityId = entityId,
                Changes = JsonSerializer.Serialize(changes),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _dbcontext.ActivityLogs.Add(log);
            await _dbcontext.SaveChangesAsync();
        }

    }
}