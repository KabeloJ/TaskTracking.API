using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace TaskTracking.UI.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        private readonly IConfiguration _config;
        public RegisterModel( IConfiguration config)
        {
            _config = config;
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                using HttpClient client = new();
                string APIURL = _config.GetSection("APIURL").Value;
                var response = client.PostAsJsonAsync($"{APIURL}auth/register", new { Username, Email, Password }).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<string>();
                    return RedirectToPage("/Login");
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                    ModelState.AddModelError("", error.Message);
                    return Page();
                }
            }
            return Page();
        }
    }
    public class ErrorResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
