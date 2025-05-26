using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskTracking.Application.Services;
using TaskTracking.Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskTracking.API.BackgroundServices
{
    public class TaskStatusScheduler : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TaskStatusScheduler> _logger; 

        private Timer _timer;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Default: Runs every hour

        public TaskStatusScheduler(IServiceScopeFactory scopeFactory, ILogger<TaskStatusScheduler> logger, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            // Read interval from config
            var intervalMinutes = configuration.GetValue<int>("SchedulerSettings:RunIntervalMinutes");
            _interval = TimeSpan.FromMinutes(intervalMinutes);

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Task Status Scheduler started. Running every {_interval.TotalMinutes} minutes.");
            _timer = new Timer(UpdateOverdueTasks, null, TimeSpan.Zero, _interval);
            return Task.CompletedTask;
        }

        private void UpdateOverdueTasks(object state)
        {
            using var scope = _scopeFactory.CreateScope();
            var taskService = scope.ServiceProvider.GetRequiredService<TaskService>();

            try
            {
                var affectedTasks = taskService.MarkOverdueTasksAsync().Result;
                _logger.LogInformation($"Updated {affectedTasks} overdue tasks.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating overdue tasks.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task Status Scheduler stopped.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}