using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }

    }

    public enum TaskStatus
    {
        New,
        InProgress,
        Completed,
        Delayed,
        Overdue
    }

}
