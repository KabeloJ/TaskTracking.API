using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text;
using TaskTracking.Core.Entities;

namespace TaskTracking.UI.Pages
{
    public class AddTaskModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Title is required.")]
        public string? Title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Due Date is required.")]
        [FutureDate(ErrorMessage = "Due Date cannot be in the past.")]
        public DateTime? DueDate { get; set; }


        private readonly ILogger<AddTaskModel> _logger;

        public AddTaskModel(ILogger<AddTaskModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44375/api/tasks/create");
                string? jwtToken = HttpContext.Session.GetString("JwtToken");
                request.Headers.Add("Authorization", $"Bearer {jwtToken}");

                var taskPayload = new
                {
                    Title = Title,
                    Description = Description,
                    DueDate = DueDate
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(taskPayload), Encoding.UTF8, "application/json");
                request.Content = jsonContent;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string msg = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Content("<script>window.parent.location.href='Index';</script>", "text/html");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create task.");
                    return Page();
                }
            }
            return Page();
        }
    }
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime < DateTime.UtcNow)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
