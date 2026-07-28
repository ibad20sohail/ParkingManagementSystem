using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PMS.Application.IServices;
using PMS.Application.Models.Responses.User;
using PMS.Application.Settings;
using System.Security.Claims;

namespace PMS.Infrastructure.Services
{
    public class CookieAuthenticationService : ICookieAuthenticationService
    {
        private readonly CookieSettings _settings;
        public CookieAuthenticationService(IOptions<ApplicationParameters> settings)
        {
            _settings = settings.Value.CookieSettings;
        }
        public async Task SignInAsync(HttpContext httpContext, LoginUserResponse user)
        {
            var claims = new List<Claim>
            {
                new Claim("user_id", user.UserId.ToString()!),
                new Claim(ClaimTypes.Name,user.UserName!),
                new Claim(ClaimTypes.Role, user.RoleName!),
                new Claim("role_id", user.RoleId.ToString()!)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(_settings.Expiration)
            };

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
