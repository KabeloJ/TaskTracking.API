using Microsoft.EntityFrameworkCore;
using TaskTracking.Infrastructure.Data;
using TaskTracking.Infrastructure;
using TaskTracking.API;
using TaskTracking.Application;
using TaskTracking.API.BackgroundServices;
using TaskTracking.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAuthenticationService(builder.Configuration);


//Background task
builder.Services.AddHostedService<TaskStatusScheduler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
});



var app = builder.Build();

/*
 * Trigger scheduled interval manual with code below
using var scope = app.Services.CreateScope();
var taskService = scope.ServiceProvider.GetRequiredService<TaskService>();
await taskService.MarkOverdueTasksAsync();
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
