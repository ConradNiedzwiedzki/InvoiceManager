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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [Display(Name = "Nazwa u�ytkownika")]
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

            [Display(Name = "Numer ksi�gowego")]
            public string AccountantId { get; set; }

            [Display(Name = "Nazwa Twojej firmy")]
            public string UserCompanyName { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Username = user.UserName;

            Input = new InputModel
            {
                Email = user.Email,
                AccountantId = user.AccountantId,
                UserCompanyName = user.UserCompanyName
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (Input.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            if (Input.AccountantId != user.AccountantId)
            {
                var setAccountIdResult = SetAccountantIdAsync(user, Input.AccountantId);
                if (!setAccountIdResult.IsCompletedSuccessfully)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            if (Input.UserCompanyName != user.UserCompanyName)
            {
                var setUserCompanyNameResult = SetUserCompanyNameAsync(user, Input.UserCompanyName);
                if (!setUserCompanyNameResult.IsCompletedSuccessfully)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            StatusMessage = "Tw�j profil zosta� zaktualizowany";
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(user.Email, callbackUrl);

            StatusMessage = "Email weryfikacyjny zosta� wys�any. Sprawd� swoj� skrzynk�.";
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