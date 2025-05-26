using TaskTracking.Core.Entities;
using System.Threading.Tasks;

namespace TaskTracking.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem> AddTaskAsync(TaskItem task);
        Task<List<TaskItem>> GetTasksByUserIdAsync(int userId);
        Task<TaskItem> GetTaskByIdAsync(int taskId);
        bool UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(int taskId);
        Task<List<TaskItem>> GetAllTasks();
        Task<List<TaskItem>> GetOverdueTasksAsync();
        Task<List<TaskItem>> GetTasksAsync(Entities.TaskStatus? status, DateTime? dueBefore, int? assignedToId);
    }
}