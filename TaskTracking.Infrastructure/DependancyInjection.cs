using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracking.Core.Interfaces;
using TaskTracking.Infrastructure.Data;
using TaskTracking.Infrastructure.Repositories;

namespace TaskTracking.Infrastructure
{
    public static class DependancyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TaskTrackingDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();

        }
    }
}
