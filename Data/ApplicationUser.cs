using Microsoft.AspNetCore.Identity;

namespace InvoiceManager.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string AccountantId { get; set; }
        public string UserCompanyName { get; set; }
    }
}
