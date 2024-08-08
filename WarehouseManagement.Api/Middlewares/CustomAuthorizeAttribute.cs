using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using WarehouseManagement.Core.Contracts;

namespace WarehouseManagement.Api.Middlewares
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata
                .OfType<IAllowAnonymous>()
                .Any();

            if (allowAnonymous)
            {
                // Skip authorization if AllowAnonymous is present
                return;
            }

            // In the future maybe replace with context.RouteData to access controller and action
            var serviceProvider = context.HttpContext.RequestServices;
            var roleService = serviceProvider.GetRequiredService<IRoleService>();

            string action = context.RouteData.Values["action"]!.ToString()!;
            string controller = context.RouteData.Values["controller"]!.ToString()!;

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
