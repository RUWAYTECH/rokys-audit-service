using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rokys.Audit.WebAPI.Services;
using System.Security.Claims;

namespace Rokys.Audit.WebAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _permission;
        private readonly string _resource;

        public RequirePermissionAttribute(string permission, string resource = "")
        {
            _permission = permission;
            _resource = resource;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var identityServerService = context.HttpContext.RequestServices.GetService<IIdentityServerService>();
            if (identityServerService == null)
            {
                context.Result = new StatusCodeResult(500); // Internal server error
                return;
            }

            // Get user ID from claims
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                        context.HttpContext.User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check permission
            var hasPermission = await identityServerService.HasPermissionAsync(userId, _permission, _resource);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

    // Extension methods for easier usage
    public static class PermissionConstants
    {
        public const string CREATE = "CREATE";
        public const string READ = "READ";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";
        public const string APPROVE = "APPROVE";
        public const string SIGN = "SIGN";
        public const string FORWARD = "FORWARD";
        public const string RETURN = "RETURN";
    }

    public static class ResourceConstants
    {
        public const string USERS = "USERS";
        public const string EMPLOYEES = "EMPLOYEES";
        public const string MEMOS = "MEMOS";
        public const string REPORTS = "REPORTS";
        public const string DASHBOARD = "DASHBOARD";
        public const string ROLES = "ROLES";
        public const string PERMISSIONS = "PERMISSIONS";
    }
}