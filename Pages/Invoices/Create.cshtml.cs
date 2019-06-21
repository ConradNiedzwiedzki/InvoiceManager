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
        public CreateModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IActionResult OnGet()
        {
            Invoice = new Invoice
            {
                CompanyName = "Nazwa firmy",
                IssueDate = DateTime.Today,
                Amount = 1000.00,
                DueDate = DateTime.Today.AddDays(1)
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

            Invoice.OwnerId = UserManager.GetUserId(User);
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