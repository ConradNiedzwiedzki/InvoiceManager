using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using InvoiceManager.Authorization;
using InvoiceManager.Models;

namespace InvoiceManager.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var adminId = await EnsureUser(serviceProvider, testUserPw, "admin@niedzwiedzki.net");
                await EnsureRole(serviceProvider, adminId, Constants.InvoiceAdministratorsRole);

                var managerId = await EnsureUser(serviceProvider, testUserPw, "manager@niedzwiedzki.net");
                await EnsureRole(serviceProvider, managerId, Constants.InvoiceAccountantRole);

                SeedDb(context, adminId);
            }
        }
      
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string userName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return user.Id;
            }

            user = new ApplicationUser { UserName = userName };
            await userManager.CreateAsync(user, testUserPw);

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                _ = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(uid);
            var ir = await userManager.AddToRoleAsync(user, role);

            return ir;
        }

        public static void SeedDb(ApplicationDbContext context, string adminId)
        {
            if (context.Invoice.Any())
            {
                return;
            }

            context.Invoice.AddRange(
            new Invoice
            {
                CompanyName = "Asseco",
                DueDate = new DateTime(2019, 5, 15),
                IssueDate = new DateTime(2019, 5, 20),
                Amount = 1234.40,
                Status = InvoiceStatus.Approved,
                OwnerId = adminId
            },

            new Invoice
            {
                CompanyName = "KMD Poland Sp. z. o. o.",
                DueDate = new DateTime(2019, 5, 3),
                IssueDate = new DateTime(2019, 5, 13),
                Amount = 1234.40,
                Status = InvoiceStatus.Submitted,
                OwnerId = adminId
            },

            new Invoice
            {
                CompanyName = "Paxer",
                DueDate = new DateTime(2019, 5, 8),
                IssueDate = new DateTime(2019, 5, 15),
                Amount = 1234.40,
                Status = InvoiceStatus.Approved,
                OwnerId = adminId
            }
            );

            context.SaveChanges();
        }
    }
}