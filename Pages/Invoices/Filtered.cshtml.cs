using System;
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
    public class FilteredModel : DiBasePageModel
    {
        public FilteredModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IList<Invoice> Invoice { get; set; }

        public DateTime FilterDate { get; set; }

        public async Task OnGetAsync(DateTime date)
        {
            var allInvoices = from i in Context.Invoice
                select i;

            IQueryable<Invoice> filteredInvoices = null;

            var isAuthorized = User.IsInRole(Resources.ApplicationTexts.InvoiceAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (isAuthorized)
            {
                filteredInvoices = allInvoices.Where(i => i.IssueDate == date && (i.AccountantId == currentUserId || i.OwnerId == currentUserId));
            }

            foreach (var invoice in allInvoices)
            {
                if (string.IsNullOrEmpty(invoice.AccountantId))
                {
                    invoice.AccountantId =
                        "<Nie dodałeś jeszcze ID księgowego do swojego profilu Dodaj ID księgowego, aby on również zobaczył fakturę!";
                }

                if (string.IsNullOrEmpty(invoice.CompanyName))
                {
                    invoice.CompanyName =
                        "<Nie dodałeś jeszcze nazwy swojej firmy do swojego profilu!>";
                }
            }

            FilterDate = date;
            Invoice = await filteredInvoices.ToListAsync();
        }
    }
}
