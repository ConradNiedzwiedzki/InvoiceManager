using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using InvoiceManager.Authorization;
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
            var invoices = from i in Context.Invoice
                            select i;

            var isAuthorized = User.IsInRole(Constants.InvoiceManagersRole) || User.IsInRole(Constants.InvoiceAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized)
            {
                invoices = invoices.Where(i => i.Status == InvoiceStatus.Approved || i.OwnerId == currentUserId);
            }

            Invoice = await invoices.ToListAsync();
        }
    }
}
