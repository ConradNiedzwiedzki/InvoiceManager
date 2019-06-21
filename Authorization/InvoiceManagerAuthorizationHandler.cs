using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

using InvoiceManager.Models;

namespace InvoiceManager.Authorization
{
    public class InvoiceManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Invoice resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.ApproveOperationName && requirement.Name != Constants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Constants.InvoiceManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
