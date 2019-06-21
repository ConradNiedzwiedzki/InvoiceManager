using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace InvoiceManager.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string AccountantId { get; set; }
        public string UserCompanyName { get; set; }
    }
}
