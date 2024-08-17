using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Api.Extensions;

public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseManagementDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}