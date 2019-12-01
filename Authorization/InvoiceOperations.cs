using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace InvoiceManager.Authorization
{
    public static class InvoiceOperations
    {
        public static OperationAuthorizationRequirement Create = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.CreateOperationName };
        public static OperationAuthorizationRequirement Read = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.ReadOperationName };
        public static OperationAuthorizationRequirement Update = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.UpdateOperationName };
        public static OperationAuthorizationRequirement Delete = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.DeleteOperationName };
        public static OperationAuthorizationRequirement Approve = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.ApproveOperationName };
        public static OperationAuthorizationRequirement Reject = new OperationAuthorizationRequirement { Name = Resources.ApplicationTexts.RejectOperationName };
    }
}