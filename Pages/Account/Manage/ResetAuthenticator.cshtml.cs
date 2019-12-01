using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using InvoiceManager.Data;

namespace InvoiceManager.Pages.Account.Manage
{
    public class ResetAuthenticatorModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ResetAuthenticatorModel> logger;

        public ResetAuthenticatorModel(UserManager<ApplicationUser> userManager, ILogger<ResetAuthenticatorModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnableToLoadUserWithId, userManager.GetUserId(User)));
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnableToLoadUserWithId, userManager.GetUserId(User)));
            }

            await userManager.SetTwoFactorEnabledAsync(user, false);
            await userManager.ResetAuthenticatorKeyAsync(user);
            logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToPage("./EnableAuthenticator");
        }
    }
}