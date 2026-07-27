using Microsoft.AspNetCore.Http;
using PMS.Application.Models.Responses.User;

namespace PMS.Application.IServices
{
    public interface ICookieAuthenticationService
    {
        Task SignInAsync(HttpContext httpContext, LoginUserResponse user);
        Task SignOutAsync(HttpContext httpContext);
    }
}
