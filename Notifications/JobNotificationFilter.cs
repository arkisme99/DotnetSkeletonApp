using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.AspNetCore.SignalR;

namespace DotnetSkeletonApp.Notifications
{
    public class JobNotificationFilter(
        IHubContext<JobNotificationHub> _hub
        ) : IServerFilter, IElectStateFilter
    {
        // ✅ SUCCESS / ❌ FAILED
        public void OnPerformed(PerformedContext context)
        {
            // Ambil dari connection storage
            var userId = context.Connection.GetJobParameter(context.BackgroundJob.Id, "CreatorUserId");

            if (context.Exception == null)
            {
                _hub.Clients.Group(userId).SendAsync("JobSuccess", new
                {
                    JobId = context.BackgroundJob.Id,
                    Job = context.BackgroundJob.Job.Type.Name
                });
            }
            else
            {
                _hub.Clients.Group(userId).SendAsync("JobFailed", new
                {
                    JobId = context.BackgroundJob.Id,
                    Error = context.Exception.Message
                });
            }
        }

        public void OnPerforming(PerformingContext context) { }

        // 🔁 RETRY
        public void OnStateElection(ElectStateContext context)
        {
            // Ambil dari connection storage
            var userId = context.Connection.GetJobParameter(context.BackgroundJob.Id, "CreatorUserId");

            if (context.CandidateState is FailedState failed &&
                failed.Exception != null)
            {
                _hub.Clients.Group(userId).SendAsync("JobRetry", new
                {
                    JobId = context.BackgroundJob.Id,
                    Error = failed.Exception.Message,
                    // RetryCount = failed.RetryCount
                });
            }
        }
    }
}