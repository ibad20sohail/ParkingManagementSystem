using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using PMS.Application.IServices;
using PMS.Application.Models.Responses;
using System.Security.Claims;

namespace PMS.Infrastructure.Services
{
    public class CookieAuthenticationService : ICookieAuthenticationService
    {
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
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal,properties);
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
