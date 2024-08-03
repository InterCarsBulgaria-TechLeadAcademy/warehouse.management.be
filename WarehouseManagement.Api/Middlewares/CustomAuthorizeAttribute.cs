using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using WarehouseManagement.Core.Services;

namespace WarehouseManagement.Api.Middlewares
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string action;
        private readonly string controller;

        public CustomAuthorizeAttribute(string action, string controller)
        {
            this.action = action;
            this.controller = controller;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // In the future maybe replace with context.RouteData to access controller and action
            var serviceProvider = context.HttpContext.RequestServices;
            var roleService = serviceProvider.GetRequiredService<RoleService>();

            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
                return;
            }

            var roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            foreach (var role in roles)
            {
                if (await roleService.CheckRoleAccessAsync(role, action, controller))
                {
                    return;
                }
            }

            context.Result = new ForbidResult();
        }
    }
}
