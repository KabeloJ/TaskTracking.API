using TaskTracking.Core.Entities;
using TaskTracking.Core.Interfaces;
using TaskTracking.Infrastructure.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TaskTracking.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskTrackingDbContext _dbContext;

        public TaskRepository(TaskTrackingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();
            return task;
        }
        public async Task<List<TaskItem>> GetTasksAsync(Core.Entities.TaskStatus? status = null, DateTime? dueBefore = null, int? assignedToId = null)
        {
            var query = _dbContext.Tasks.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status);
            }

            if (dueBefore.HasValue)
            {
                query = query.Where(t => t.DueDate < dueBefore); 
            }

            if (assignedToId != null && assignedToId > 0)
            {
                query = query.Where(x => x.AssignedUserId == assignedToId);
            }

            return await query.ToListAsync();
        }
        public async Task<List<TaskItem>> GetTasksByUserIdAsync(int userId)
        {
            return await _dbContext.Tasks
                .Where(t => t.AssignedUserId == userId)
                .ToListAsync();
        }
        //public async Task<TaskItem> GetTaskByIdAsync(int taskId)
        //{
        //    return await _dbContext.Tasks.Where(x=>x.Id == taskId).Include(x=>x.AssignedUser).FirstAsync();
        //}

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            return await _dbContext.Tasks
                .Where(x => x.Id == taskId)
                .Select(task => new TaskItem
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    AssignedUserId = task.AssignedUserId,
                    AssignedUser = task.AssignedUser != null ? new User
                    {
                        Id = task.AssignedUser.Id,
                        Username = task.AssignedUser.Username,
                        Email = task.AssignedUser.Email
                    } : null
                })
                .FirstOrDefaultAsync();
        }

        public bool UpdateTaskAsync(TaskItem task)
        {
            using (var db = new TaskTrackingDbContext())
            {
                var target = db.Tasks.Find(task.Id);
                db.Entry(target).CurrentValues.SetValues(task);
                int v = db.SaveChanges();
                return v > 0;
            }
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var task = await _dbContext.Tasks.FindAsync(taskId);
            if (task == null) return false;

            _dbContext.Tasks.Remove(task);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<TaskItem>> GetAllTasks()
        {
            return await _dbContext.Tasks.ToListAsync();
        }
        public async Task<List<TaskItem>> GetOverdueTasksAsync()
        {
            return await _dbContext.Tasks.Where(t => t.DueDate < DateTime.Now 
                                                    && t.Status != Core.Entities.TaskStatus.Completed).ToListAsync();
        }
    }
}