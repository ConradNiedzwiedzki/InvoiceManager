using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using InvoiceManager.Authorization;
using InvoiceManager.Data;
using InvoiceManager.Models;
using System.ComponentModel.DataAnnotations;

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
            var invoices = from i in Context.Invoice
                select i;

            var isAuthorized = User.IsInRole(Constants.InvoiceAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized)
            {
                invoices = invoices.Where(i => i.IssueDate == date && (i.AccountantId == currentUserId || i.OwnerId == currentUserId));
            }

            foreach (var invoice in invoices)
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
            Invoice = await invoices.ToListAsync();
        }
    }
}
