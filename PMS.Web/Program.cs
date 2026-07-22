using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using ParkingManagementSystem.ExceptionHandler;
using ParkingManagementSystem.Filters;
using PMS.Application.IServices;
using PMS.Infrastructure.DependencyInjection;
using PMS.Infrastructure.Services;
using Serilog;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

//Serilog Configuration
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

//Cookie authentication configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";

        options.Cookie.Name = "PMS.Auth";

        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;

        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalLoggingFilter>();
});

//DB Injection
builder.Services.AddScoped<IDbConnection>(_ => new SqlConnection(builder.Configuration.GetConnectionString("Connection")));

//Repository Injection
builder.Services.AddRepositories();

//Services Injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICookieAuthenticationService, CookieAuthenticationService>();

//Global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseExceptionHandler();  //Global exception handler

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
