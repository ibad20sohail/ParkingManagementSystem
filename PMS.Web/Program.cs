using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ParkingManagementSystem.ExceptionHandler;
using ParkingManagementSystem.Filters;
using PMS.Application.Constants;
using PMS.Application.IServices;
using PMS.Application.Settings;
using PMS.Infrastructure.DependencyInjection;
using PMS.Infrastructure.Services;
using Serilog;
using System.Data;

//Important for repository response mapping
DefaultTypeMap.MatchNamesWithUnderscores = true;    

var builder = WebApplication.CreateBuilder(args);

//Serilog Configuration
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

//App parameters configuration
builder.Services.Configure<ApplicationParameters>(builder.Configuration.GetSection(Cons.AppPara));
var appPara = builder.Configuration.GetSection(Cons.AppPara).Get<ApplicationParameters>()!;

//Cookie authentication configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = appPara.CookieSettings.LoginPath;
        options.LogoutPath = appPara.CookieSettings.LogoutPath;
        options.AccessDeniedPath = appPara.CookieSettings.AccessDeniedPath;

        options.Cookie.Name = appPara.CookieSettings.Name;

        options.ExpireTimeSpan = TimeSpan.FromHours(appPara.CookieSettings.Expiration);
        options.SlidingExpiration = appPara.CookieSettings.SlidingExpiration;

        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.Filters.Add<GlobalLoggingFilter>();
});

//DB Injection
builder.Services.AddScoped<IDbConnection>(_ => new SqlConnection(builder.Configuration.GetConnectionString("Connection")));

//Repository Injection
builder.Services.AddRepositories();

//Services Injection
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddTransient<ICookieAuthenticationService, CookieAuthenticationService>();
builder.Services.AddTransient<IEmailService, EmailService>();

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
