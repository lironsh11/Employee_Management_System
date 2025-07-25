using EmployeeManagementSystem.Middleware;
using EmployeeManagementSystem.Repositories;
using EmployeeManagementSystem.Services;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddContainerAuth(builder.Configuration);

builder.Services.AddScoped<IEmployeeRepository, JsonEmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, JsonDepartmentRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

// ✅ Set default culture to en-US (for correct currency formatting with $)
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddContainerAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();

        // Add other services as needed
        services.AddLogging();

        return services;
    }
}
