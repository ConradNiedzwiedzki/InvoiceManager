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
    public class DeleteModel : DiBasePageModel
    {
        public DeleteModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
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

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice,InvoiceOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Invoice = await Context.Invoice.FindAsync(id);

            var invoice = await Context.Invoice.AsNoTracking().FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, invoice, InvoiceOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Context.Invoice.Remove(Invoice);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
