using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WarehouseManagement.Infrastructure.Data.Models;

namespace WarehouseManagement.Infrastructure.Extensions;

public static class RoleSeederExtensions
{
    public static async Task SeedRolesAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var roles = new string[] { "Admin" };

        foreach (var role in roles)
        {
            var exists = await roleManager.RoleExistsAsync(role);

            if (!exists)
            {
                await roleManager.CreateAsync(new ApplicationRole() { Name = role });
            }
        }
    }
}