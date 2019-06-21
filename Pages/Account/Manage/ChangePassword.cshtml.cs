using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using InvoiceManager.Data;

namespace InvoiceManager.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Pole wymagane")]
            [DataType(DataType.Password)]
            [Display(Name = "Obecne has�o")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Pole wymagane")]
            [StringLength(100, ErrorMessage = "Liczba znak�w {0} musi wynosi� przynajmniej {2} i maksymalnie {1}.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nowe has�o")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potwierd� nowe has�o")]
            [Compare("NewPassword", ErrorMessage = "Nowe has�o i potwierdzenie nowego has�a nie pasuj� do siebie.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

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

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Twoje has�o zosta�o zmienione.";

            return RedirectToPage();
        }
    }
}
