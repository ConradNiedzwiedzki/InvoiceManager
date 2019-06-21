using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InvoiceManager.Pages
{
    [AllowAnonymous]
    public class AboutModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
