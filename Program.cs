using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Services;
using VehicleManagementSystem.Services.Implementations;
using VehicleManagementSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173"
            )
            .AllowCredentials();
    });
});

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionPostgres"))
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IVendorService, VendorsService>();
builder.Services.AddScoped<IPartsService, PartsService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IEmailService,EmailService>();
builder.Services.AddScoped<IPredictionService,PredictionService>();
builder.Services.AddHostedService<CreditReminderService>();
builder.Services.AddScoped<IAdminRecommendationService, AdminRecommendationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRoles(roleManager);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await AdminSeeder.SeedAdmin(userManager);
}

app.UseSwagger();

app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    options.WithTitle("yahNepal API");
});

app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar/v1");
    return Task.CompletedTask;
});


app.UseCors("EnableCORS");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();