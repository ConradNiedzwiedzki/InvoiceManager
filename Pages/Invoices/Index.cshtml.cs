using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using InvoiceManager.Data;
using InvoiceManager.Models;

namespace InvoiceManager.Pages.Invoices
{
    public class IndexModel : DiBasePageModel
    {
        public IndexModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IList<Invoice> Invoice { get; set; }

        public async Task OnGetAsync()
        {
            var invoices = from invoice in Context.Invoice select invoice;

            var isAuthorized = User.IsInRole(Resources.ApplicationTexts.InvoiceAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized)
            {
                invoices = invoices.Where(i => i.AccountantId == currentUserId || i.OwnerId == currentUserId);
            }

            foreach (var invoice in invoices)
            {
                if (string.IsNullOrEmpty(invoice.AccountantId))
                {
                    invoice.AccountantId = Resources.ApplicationTexts.NoAccountantIdAdded;
                }

                if (string.IsNullOrEmpty(invoice.CompanyName))
                {
                    invoice.CompanyName = Resources.ApplicationTexts.NoCompanyNameAdded;
                }
            }

            Invoice = await invoices.ToListAsync();
        }
    }
}
