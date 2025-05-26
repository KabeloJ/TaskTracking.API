using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracking.Application.Models;
using TaskTracking.Application.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskTracking.Core.Entities;

[Route("api/tasks")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [Authorize] 
    [HttpPost("create")]
    public async Task<IActionResult> CreateTask(TaskModel request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("Invalid user session.");

        var userId = int.Parse(userIdClaim.Value);
        var task = await _taskService.CreateTaskAsync(userId, request.Title, request.Description, request.DueDate);

        return Ok(task);
    }
    [Authorize]
    [HttpGet("filtered")]
    public async Task<IActionResult> GetTasks([FromQuery] TaskTracking.Core.Entities.TaskStatus? status, [FromQuery] DateTime? dueBefore, [FromQuery] int? assignedToId)
    {
        var tasks = await _taskService.GetFilteredTasksAsync(status, dueBefore, assignedToId);
        return Ok(tasks);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetById(int taskId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("Invalid user session.");
        }

        var userId = int.Parse(userIdClaim.Value);
        var task = await _taskService.GetTaskByIdAsync(taskId);

        if (task == null)
        {
            return NotFound("Task not found.");
        }

        // ✅ Ensure user is assigned, created the task, or is an admin
        if (task.AssignedUserId != userId && !User.IsInRole("Admin"))
        {
            return Unauthorized("You do not have permission to view this task.");
        }

        return Ok(task);
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTaskForUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("Invalid user session.");

        var userId = int.Parse(userIdClaim.Value);
        var tasks = await _taskService.GetUserTasksAsync(userId);

        return Ok(tasks);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }
    [Authorize(Roles = "User,Admin")]
    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTask(int taskId, TaskItem updatedTask)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("Invalid user session.");

        var userId = int.Parse(userIdClaim.Value);
        var task = await _taskService.GetTaskByIdAsync(taskId);

        if (task == null || (task.AssignedUserId != userId && User.IsInRole("Admin") == false))
            return Forbid(); // Prevent non-admin users from modifying tasks that aren't theirs

        var success = await _taskService.UpdateTaskAsync(taskId, updatedTask.Title, updatedTask.Description, updatedTask.Status, updatedTask.DueDate);
        if (!success) return BadRequest();

        return Ok("Task updated successfully.");
    }

    [Authorize(Roles = "User,Admin")]
    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var task = await _taskService.GetTaskByIdAsync(taskId);

        if (task == null || (task.AssignedUserId != userId && User.IsInRole("Admin") == false))
            return Forbid(); // Only allow admins to delete any task

        var success = await _taskService.DeleteTaskAsync(taskId);
        if (!success) return BadRequest();

        return Ok("Task deleted successfully.");
    }
}