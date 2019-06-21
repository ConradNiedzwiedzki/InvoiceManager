using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using InvoiceManager.Authorization;
using InvoiceManager.Data;
using InvoiceManager.Models;

namespace InvoiceManager.Pages.Invoices
{
    public class EditModel : DiBasePageModel
    {
        public EditModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Invoice = await Context.Invoice.FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var invoice = await Context.Invoice.FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, invoice, InvoiceOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Invoice.OwnerId = invoice.OwnerId;

            if (invoice.Status == InvoiceStatus.Approved)
            {
                var canApprove = await AuthorizationService.AuthorizeAsync(User, invoice, InvoiceOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    invoice.Status = InvoiceStatus.Submitted;
                }
            }

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool InvoiceExists(int id)
        {
            return Context.Invoice.Any(e => e.InvoiceId == id);
        }
    }
}
