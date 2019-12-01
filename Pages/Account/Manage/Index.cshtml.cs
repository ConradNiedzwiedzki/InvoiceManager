using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using InvoiceManager.Data;
using InvoiceManager.Services;

namespace InvoiceManager.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        [Display(Name = "Nazwa u¿ytkownika")]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Pole wymagane")]
            [EmailAddress]
            public string Email { get; set; }

            [Display(Name = "Numer ksiêgowego")]
            public string AccountantId { get; set; }

            [Display(Name = "Nazwa Twojej firmy")]
            public string UserCompanyName { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            Username = user.UserName;

            Input = new InputModel
            {
                Email = user.Email,
                AccountantId = user.AccountantId,
                UserCompanyName = user.UserCompanyName
            };

            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (Input.Email != user.Email)
            {
                var setEmailResult = await userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            if (Input.AccountantId != user.AccountantId)
            {
                user.AccountantId = Input.AccountantId;
            }

            if (Input.UserCompanyName != user.UserCompanyName)
            {
                user.UserCompanyName = Input.UserCompanyName;
            }

            var result = await userManager.UpdateAsync(user);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            StatusMessage = "Twój profil zosta³ zaktualizowany";

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            await emailSender.SendEmailConfirmationAsync(user.Email, callbackUrl);

            StatusMessage = "Email weryfikacyjny zosta³ wys³any. SprawdŸ swoj¹ skrzynkê.";
            return RedirectToPage();
        }

        public virtual Task SetAccountantIdAsync(ApplicationUser user, string accountantId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if ((object)user == null)
                throw new ArgumentNullException(nameof(user));
            user.AccountantId = accountantId;
            return Task.CompletedTask;
        }

        public virtual Task SetUserCompanyNameAsync(ApplicationUser user, string userCompanyName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if ((object)user == null)
                throw new ArgumentNullException(nameof(user));
            user.UserCompanyName = userCompanyName;
            return Task.CompletedTask;
        }
    }
}
