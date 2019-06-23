using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using InvoiceManager.Data;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace InvoiceManager.Controllers
{
    [Route("[controller]/[action]")]
    public class InvoiceController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public InvoiceController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, ILogger<InvoiceController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Filter(DateTime date)
        {
            return RedirectToPage("/Invoices/Filtered", date);
        }
    }
}
