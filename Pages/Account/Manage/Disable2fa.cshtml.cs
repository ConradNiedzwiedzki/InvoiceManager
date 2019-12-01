using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using InvoiceManager.Data;

namespace InvoiceManager.Pages.Account.Manage
{
    public class Disable2FaModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<Disable2FaModel> logger;

        public Disable2FaModel(UserManager<ApplicationUser> userManager, ILogger<Disable2FaModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnableToLoadUserWithId,userManager.GetUserId(User)));
            }

            if (!await userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.CannotDisable2FA, userManager.GetUserId(User)));
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

            var disable2FaResult = await userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2FaResult.Succeeded)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnexpectedErrorWhenDisabling2FA, userManager.GetUserId(User)));
            }

            logger.LogInformation(string.Format(Resources.ApplicationTexts.UserDisabled2FA, userManager.GetUserId(User)));

            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}