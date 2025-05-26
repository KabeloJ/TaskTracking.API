using TaskTracking.Core.Entities;
using TaskTracking.Core.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace TaskTracking.Application.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<TaskItem> CreateTaskAsync(int userId, string title, string description, DateTime dueDate)
        {
            var task = new TaskItem
            {
                Title = title,
                Description = description,
                Status = Core.Entities.TaskStatus.New,
                DueDate = dueDate,
                CreatedDate = DateTime.UtcNow,
                AssignedUserId = userId
            };

            return await _taskRepository.AddTaskAsync(task);
        }
        public async Task<TaskItem> GetTaskByIdAsync(int taskId)
        {
            return await _taskRepository.GetTaskByIdAsync(taskId);
        }

        public async Task<List<TaskItem>> GetUserTasksAsync(int userId)
        {
            return await _taskRepository.GetTasksByUserIdAsync(userId);
        }
        public async Task<bool> UpdateTaskAsync(int taskId, string title, string description, Core.Entities.TaskStatus status, DateTime dueDate)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null) return false;

            task.Title = title;
            task.Description = description;
            task.Status = status;
            task.DueDate = dueDate;

            return _taskRepository.UpdateTaskAsync(task);
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            return await _taskRepository.DeleteTaskAsync(taskId);
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllTasks();
        }
        
        //public async Task<int> MarkOverdueTasksAsync()
        //{
        //    var overdueTasks = await _taskRepository.GetOverdueTasksAsync();
        //    int updatedCount = 0;

        //    foreach (var task in overdueTasks)
        //    {
        //        var retryPolicy = Policy
        //            .Handle<Exception>()
        //            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt * 2), (exception, timeSpan, retryCount, context) =>
        //            {
        //                _logger.LogWarning($"Retry {retryCount}: Error updating task {task.Id}. Waiting {timeSpan} before retry.");
        //            });

        //        var success = await retryPolicy.ExecuteAsync(async () =>
        //        {
        //            task.Status = Core.Entities.TaskStatus.Overdue;
        //            return _taskRepository.UpdateTaskAsync(task);
        //        });

        //        if (success) updatedCount++;
        //    }

        //    _logger.LogInformation($"Successfully updated {updatedCount}/{overdueTasks.Count} overdue tasks.");
        //    return updatedCount;
        //}
        public async Task<int> MarkOverdueTasksAsync()
        {
            var overdueTasks = await _taskRepository.GetOverdueTasksAsync();
            int updatedCount = 0;

            foreach (var task in overdueTasks)
            {
                task.Status = Core.Entities.TaskStatus.Overdue;
                var success = _taskRepository.UpdateTaskAsync(task);

                if (success) updatedCount++;
            }

            _logger.LogInformation($"Successfully updated {updatedCount}/{overdueTasks.Count} overdue tasks.");
            return updatedCount;
        }
        public async Task<List<TaskItem>> GetFilteredTasksAsync(Core.Entities.TaskStatus? status, DateTime? dueBefore, int? assignedToId)
        {
            return await _taskRepository.GetTasksAsync(status, dueBefore, assignedToId);
        }
    }
}