using System;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using InvoiceManager.Data;

namespace InvoiceManager.Controllers
{
    [Route("[controller]/[action]")]
    public class InvoiceController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger logger;
        private readonly ApplicationDbContext context;

        public InvoiceController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, ILogger<InvoiceController> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
            this.context = context;
        }

        public IActionResult Filter(DateTime date)
        {
            return RedirectToPage("/Invoices/Filtered", date);
        }
    }
}
