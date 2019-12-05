using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using InvoiceManager.Data;
using InvoiceManager.Services;

namespace InvoiceManager.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<LoginModel> logger;
        private readonly IEmailSender emailSender;

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Pole wymagane")]
            [EmailAddress]
            [Display(Name = "Adres e-mail")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Pole wymagane")]
            [StringLength(100, ErrorMessage = "Liczba znaków {0} musi wynosiæ przynajmniej {2} i maksymalnie {1}.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Has³o")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "PotwierdŸ has³o")]
            [Compare("Password", ErrorMessage = "Nowe has³o i potwierdzenie nowego has³a nie pasuj¹ do siebie.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Nazwa Twojej firmy")]
            public string UserCompanyName { get; set; }

            [Display(Name = "Identyfikator ksiêgowego (wype³nij, je¿eli ju¿ go posiadasz)")]
            public string AccountantId { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, AccountantId = Input.AccountantId, UserCompanyName = Input.UserCompanyName};
            var result = await userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                logger.LogInformation(Resources.ApplicationTexts.UserCreatedNewAccountWithPassword);

                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                await emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

                await signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(Url.GetLocalUrl(returnUrl));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
