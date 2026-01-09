using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DotnetSkeletonApp.Services;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.AspNetCore.SignalR;

namespace DotnetSkeletonApp.Notifications
{
    public class JobNotificationFilter(
        IServiceScopeFactory _scopeFactory
        ) : IApplyStateFilter
    {
        public async void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            using var scope = _scopeFactory.CreateScope();
            var _notifService = scope.ServiceProvider.GetRequiredService<NotificationService>();

            var jobId = context.BackgroundJob.Id;
            var userId = context.Connection.GetJobParameter(jobId, "CreatorUserId");
            var jobName = context.Connection.GetJobParameter(jobId, "JobNameNew");

            // Logika berdasarkan perubahan status
            if (context.NewState is SucceededState)
            {
                await _notifService.AddNotificationAsync(userId, $"Proccess {jobName} Success", null, "JobSuccess");
            }
            else if (context.NewState is FailedState failedState)
            {
                await _notifService.AddNotificationAsync(userId, $"Proccess {jobName} Failed: {failedState.Exception.Message}", null, "JobFailed");
            }
            else if (context.NewState is ScheduledState && context.OldStateName == FailedState.StateName)
            {
                await _notifService.AddNotificationAsync(userId, $"Retry Proccess {jobName}", null, "JobRetry");
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            // Tidak perlu diisi jika hanya ingin mencatat status baru
        }
        /* // ✅ SUCCESS / ❌ FAILED
        public async void OnPerformed(PerformedContext context)
        {
            // Ambil dari connection storage
            var jobId = context.BackgroundJob.Id;
            var userId = context.Connection.GetJobParameter(jobId, "CreatorUserId");
            var jobName = context.Connection.GetJobParameter(jobId, "JobNameNew");

            if (context.Exception == null)
            {
                await _notifService.AddNotificationAsync(userId, $"Proccess {jobName} Success", null, "JobSuccess");
            }
            else
            {
                await _notifService.AddNotificationAsync(userId, $"Proccess {jobName} Failed", null, "JobFailed");
            }
        }

        public void OnPerforming(PerformingContext context) { }

        // 🔁 RETRY
        public async void OnStateElection(ElectStateContext context)
        {
            // Tidak dipakai deh karena retry, cukup sukses / failed saja kayanya
            var jobId = context.BackgroundJob.Id;
            var userId = context.Connection.GetJobParameter(jobId, "CreatorUserId");
            var jobName = context.Connection.GetJobParameter(jobId, "JobNameNew");

            if (context.CandidateState is FailedState failed &&
                failed.Exception != null)
            {
                // _hub.Clients.Group(userId).SendAsync("JobRetry", new
                // {
                //     JobId = context.BackgroundJob.Id,
                //     Error = failed.Exception.Message,
                //     RetryCount = failed.RetryCount
                // });

                // await _notifService.AddNotificationAsync(userId, $"Proccess Retry {jobName}", null, "JobRetry");
            }
        } */
    }
}