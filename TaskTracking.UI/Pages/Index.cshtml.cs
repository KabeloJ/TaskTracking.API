using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TaskTracking.Core.Entities;
using TaskTracking.UI.Models;

namespace TaskTracking.UI.Pages
{
    public class IndexModel : PageModel
    {
        public UserClaimsModel? UserClaimsModel { get; set; }
        public List<TaskItem>? Tasks { get; set; }
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _config;
        public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task OnGetAsync()
        {
            string? jwtToken = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrWhiteSpace(jwtToken))
            {
                var JwtSettings = _config.GetSection("JwtSettings");
                var principal = JwtHelper.GetClaimsPrincipal(jwtToken, JwtSettings);
                UserClaimsModel = JwtHelper.GetUserClaims(principal);
                ViewData["UserClaims"] = UserClaimsModel;
                await GetTasks(jwtToken);
            }
            else
            {
                Response.Redirect("/Login");
            }
        }
        async Task GetTasks(string jwtToken)
        {
            var client = new HttpClient();
            string? requestUrl = null;
            if (UserClaimsModel.Role == "Admin")
            {
                requestUrl = $"https://localhost:44375/api/tasks/all";
            }
            else
            {
                requestUrl = $"https://localhost:44375/api/tasks?status={TaskTracking.Core.Entities.TaskStatus.Overdue}&dueBefore={DateTime.UtcNow:yyyy-MM-dd}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", $"Bearer {jwtToken}");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            Tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();

        }
    }
}
