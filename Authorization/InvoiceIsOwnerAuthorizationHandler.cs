﻿using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

using InvoiceManager.Data;
using InvoiceManager.Models;

namespace InvoiceManager.Authorization
{
    public class InvoiceIsOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public InvoiceIsOwnerAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Invoice resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Resources.ApplicationTexts.CreateOperationName 
                && requirement.Name != Resources.ApplicationTexts.ReadOperationName 
                && requirement.Name != Resources.ApplicationTexts.UpdateOperationName 
                && requirement.Name != Resources.ApplicationTexts.DeleteOperationName )
            {
                return Task.CompletedTask;
            }

            if (resource.OwnerId == userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
