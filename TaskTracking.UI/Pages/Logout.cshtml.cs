using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TaskTracking.UI.Pages
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear(); 
            return RedirectToPage("/Login"); 
        }

    }
}
