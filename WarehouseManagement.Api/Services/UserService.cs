using System.Security.Claims;
using WarehouseManagement.Api.Services.Contracts;

namespace WarehouseManagement.Api.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor httpContext;

    public UserService(IHttpContextAccessor httpContext)
    {
        this.httpContext = httpContext;
    }

    public string UserId =>
        httpContext
            .HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
            ?.Value ?? "Guest";
}
