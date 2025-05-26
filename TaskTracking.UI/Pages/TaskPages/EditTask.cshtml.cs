using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using TaskTracking.Core.Entities;
using TaskTracking.UI.Models;

namespace TaskTracking.UI.Pages.TaskPages
{
    public class EditTaskModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Title is required.")]
        public string? Title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Due Date is required.")]
        public DateTime? DueDate { get; set; }

        [BindProperty]
        public string Status { get; set; }

        public string? AssignedUser { get; set; }
        [BindProperty]
        public string? AssignedUserId { get; set; }
        [BindProperty]
        public string? TaskId { get; set; }

        private readonly IConfiguration _config;
        public EditTaskModel(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IActionResult> OnGet(string id)
        {
            string? jwtToken = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwtToken))
            {
                await GetTask(jwtToken, id);
                return Page();
            }
            return NotFound();
        }

        async Task GetTask(string jwtToken, string id)
        {
            var client = new HttpClient();
            string APIURL = _config.GetSection("APIURL").Value;
            var requestUrl = $"{APIURL}tasks/{id}";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", $"Bearer {jwtToken}");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var task = await response.Content.ReadFromJsonAsync<TaskItem>();
            if (task != null)
            {
                Title = task.Title;
                Description = task.Description;
                DueDate = Convert.ToDateTime(task.DueDate.ToString("yyyy-MM-dd HH:mm"));
                AssignedUser = task.AssignedUser?.Username;
                Status = task.Status.ToString();
                TaskId = id;
            }
        }
        public async Task<IActionResult> OnPost()
        {
            var client = new HttpClient(); 
            var requestUrl = $"{_config.GetSection("APIURL").Value}tasks/{TaskId}";

            var updatedTask = new
            {
                id = this.TaskId,
                title = this.Title,
                description = this.Description,
                status = Core.Entities.TaskStatus.InProgress,
                dueDate = DueDate,
                assignedUserId = "0"
            };
            string jwtToken = HttpContext.Session.GetString("JwtToken");

            var jsonContent = new StringContent(JsonSerializer.Serialize(updatedTask), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, requestUrl)
            {
                Content = jsonContent
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.SendAsync(request);



            if (response.IsSuccessStatusCode)
                return RedirectToPage("/Index");

            ModelState.AddModelError("", "Failed to update task.");
            return Page();
        }
    }
}
