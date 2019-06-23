using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using InvoiceManager.Authorization;
using InvoiceManager.Data;
using InvoiceManager.Models;
using SQLitePCL;

namespace InvoiceManager.Pages.Invoices
{
    public class CreateModel : DiBasePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public Invoice Invoice { get; set; }

        public List<string> ListOfCompanies { get; set; }

        public IActionResult OnGet()
        {
            Invoice = new Invoice
            {
                IssueDate = DateTime.Today,
                DueDate = DateTime.Today
            };

            ListOfCompanies = (from invoice in _context.Invoice
                                where invoice.OwnerId == UserManager.GetUserId(User)
                                select invoice.CompanyName).Distinct().ToList();

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