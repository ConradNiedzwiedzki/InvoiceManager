using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using InvoiceManager.Models;

namespace InvoiceManager.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPassword)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var adminId = await EnsureUser(serviceProvider, testUserPassword, Resources.ApplicationTexts.AdminEmail);
                await EnsureRole(serviceProvider, adminId, Resources.ApplicationTexts.InvoiceAdministratorsRole);

                var managerId = await EnsureUser(serviceProvider, testUserPassword, Resources.ApplicationTexts.ManagerEmail);
                await EnsureRole(serviceProvider, managerId, Resources.ApplicationTexts.InvoiceAccountantRole);

                SeedDb(context, adminId);
            }
        }
      
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPassword, string userName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return user.Id;
            }

            user = new ApplicationUser { UserName = userName };
            await userManager.CreateAsync(user, testUserPassword);

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string userId, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                _ = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(userId);
            var identityResult = await userManager.AddToRoleAsync(user, role);

            return identityResult;
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