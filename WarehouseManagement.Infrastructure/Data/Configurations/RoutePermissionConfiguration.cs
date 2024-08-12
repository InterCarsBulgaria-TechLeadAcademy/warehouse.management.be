using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;
using WarehouseManagement.Infrastructure.Data.Models;
using static WarehouseManagement.Common.GeneralApplicationConstants;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class RoutePermissionConfiguration : BaseConfiguration<RoutePermission>, IEntityTypeConfiguration<RoutePermission>
{
    public void Configure(EntityTypeBuilder<RoutePermission> builder)
    {
        base.Configure(builder);

        var routePermissions = CreateRoutePermissions();
        builder.HasData(routePermissions);
    }

    private IEnumerable<RoutePermission> CreateRoutePermissions()
    {
        var routePermissions = new List<RoutePermission>();

        var apiAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == ApiAssemblyName);

        if (apiAssembly == null)
        {
            throw new Exception($"Assembly with name {ApiAssemblyName} was not found.");
        }

        var controllers = apiAssembly.GetTypes()
            .Where(t => !t.IsAbstract 
                && typeof(ControllerBase).IsAssignableFrom(t));

        foreach (var controller in controllers)
        {
            var controllerName = controller
                .Name
                .Replace("Controller", string.Empty);

            var methods = controller
                .GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(m => m.IsPublic && !m.IsSpecialName);

            foreach (var method in methods)
            {
                routePermissions.Add(new RoutePermission
                {
                    ControllerName = controllerName,
                    ActionName = method.Name
                });
            }
        }
        
        return routePermissions;
    }
}
