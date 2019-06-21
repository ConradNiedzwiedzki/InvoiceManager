using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

using InvoiceManager.Data;

namespace InvoiceManager.Pages.Invoices
{
    public class DiBasePageModel : PageModel
    {
        protected ApplicationDbContext Context { get; }
        protected IAuthorizationService AuthorizationService { get; }
        protected UserManager<ApplicationUser> UserManager { get; }

        public DiBasePageModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager) : base()
        {
            Context = context;
            UserManager = userManager;
            AuthorizationService = authorizationService;
        } 
    }
}
