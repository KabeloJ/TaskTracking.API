using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskTracking.Application.Services;

namespace TaskTracking.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<UserService>();
            services.AddScoped<TaskService>();
        }
    }
}
