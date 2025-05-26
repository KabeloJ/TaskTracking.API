using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TaskTracking.UI.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        private readonly IConfiguration _configuration;
        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("", "Username and Password are required.");
                return Page();
            }

            using HttpClient client = new();
            string APIURL = _configuration.GetSection("APIURL").Value;
            var response = client.PostAsJsonAsync($"{APIURL}auth/login", new { Email, Password }).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<JObject>().Result;
                HttpContext.Session.SetString("JwtToken", result.Token);
                return RedirectToPage("/Index");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return Page();
            }
        }
    }
    public class JObject
    {
        public string? Token { get; set; }
    }

}