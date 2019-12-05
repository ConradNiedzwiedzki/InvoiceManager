using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using InvoiceManager.Authorization;
using InvoiceManager.Data;
using InvoiceManager.Models;

namespace InvoiceManager.Pages.Invoices
{
    public class CreateModel : DiBasePageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public CreateModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet()
        {
            Invoice = new Invoice
            {
                IssueDate = DateTime.Today,
                DueDate = DateTime.Today
            };
            return Page();
        }

        [BindProperty]
        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnableToLoadUserWithId, userManager.GetUserId(User)));
            }

            Invoice.OwnerId = UserManager.GetUserId(User);
            Invoice.AccountantId = user.AccountantId;
            Invoice.UserCompanyName = user.UserCompanyName;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Create);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Context.Invoice.Add(Invoice);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}