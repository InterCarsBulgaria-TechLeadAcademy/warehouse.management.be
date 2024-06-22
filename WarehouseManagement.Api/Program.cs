using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Api.Extensions;
using WarehouseManagement.Api.Middlewares;
using WarehouseManagement.Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:8000");
    });
});

builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddApplicationIdentity(builder.Configuration);
builder.Services.AddApplicationService();

builder.Services.AddControllers();
builder.Services.AddDbContext<WarehouseManagementDbContext>(options => 
        options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var salesContext = scope.ServiceProvider.GetRequiredService<WarehouseManagementDbContext>();
    salesContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
