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
    public class DetailsModel : DiBasePageModel
    {
        public DetailsModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager) 
            : base(context, authorizationService, userManager)
        {
        }

        public Invoice Invoice { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Invoice = await Context.Invoice.FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (Invoice == null)
            {
                return NotFound();
            }

            var isAuthorized = User.IsInRole(Constants.InvoiceAccountantRole) || User.IsInRole(Constants.InvoiceAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized && currentUserId != Invoice.OwnerId && Invoice.Status != InvoiceStatus.Approved)
            {
                return new ChallengeResult();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, InvoiceStatus status)
        {
            var invoice = await Context.Invoice.FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var invoiceOperation = (status == InvoiceStatus.Approved) ? InvoiceOperations.Approve : InvoiceOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, invoice, invoiceOperation);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            invoice.Status = status;
            Context.Invoice.Update(invoice);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
