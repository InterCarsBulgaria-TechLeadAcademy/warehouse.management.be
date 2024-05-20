﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Common;

namespace WarehouseManagement.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            //Here we add services
            return services;
        }

        public static IServiceCollection AddApplicationDbContext(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            //Here is DBContext configurations
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<WarehouseManagementDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddScoped<IRepository, Repository>();
            return services;
        }

        public static IServiceCollection AddApplicationIdentity(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            //Here is Identity Configuration

            services
                .AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 4;
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<WarehouseManagementDbContext>();

            return services;
        }
    }
}