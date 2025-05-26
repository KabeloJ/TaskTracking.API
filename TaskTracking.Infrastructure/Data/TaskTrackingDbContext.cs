using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaskTracking.Core.Entities;

namespace TaskTracking.Infrastructure.Data
{
    public class TaskTrackingDbContext : DbContext
    {
        public TaskTrackingDbContext()
        {

        }
        public TaskTrackingDbContext(DbContextOptions<TaskTrackingDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<TaskItem>().HasKey(t => t.Id);
        }
    }

}
