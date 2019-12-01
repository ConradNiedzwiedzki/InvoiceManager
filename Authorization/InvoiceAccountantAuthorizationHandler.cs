using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

using InvoiceManager.Models;

namespace InvoiceManager.Authorization
{
    public class InvoiceAccountantAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Invoice resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Resources.ApplicationTexts.ApproveOperationName
                && requirement.Name != Resources.ApplicationTexts.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Resources.ApplicationTexts.InvoiceAccountantRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
