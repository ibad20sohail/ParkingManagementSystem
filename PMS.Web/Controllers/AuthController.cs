using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.Application.Constants;
using PMS.Application.Helper;
using PMS.Application.IServices;
using PMS.Application.Models.Requests.User;
using System.Security.Claims;

namespace PMS.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ICookieAuthenticationService _cookieAuthenticationService;
    public AuthController(IAuthService authService, ICookieAuthenticationService cookieAuthenticationService)
    {
        _authService = authService;
        _cookieAuthenticationService = cookieAuthenticationService;
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Login()
    {
        return View();
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        if (!ModelState.IsValid)
            return View();

        var result = await _authService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            TempData[Cons.Error] = result.Message;
            return View(request);
        }
        TempData[Cons.Success] = $"Welcome to PMS - {result.Model.UserName}.";
        await _cookieAuthenticationService.SignInAsync(HttpContext, result.Model);

        return RedirectToAction("Index", "Home");
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ForgetPassword()
    {
        return View();
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ForgetPassword(GetUserByUsernameRequest request)
    {
        if (!ModelState.IsValid)
            return View();

        var result = await _authService.ForgetPasswordAsync(request);

        if (!result.IsSuccess)
        {
            TempData[Cons.Error] = result.Message;
            return View(request);
        }
        TempData[Cons.Success] = $"Password reset email has been sent to {result.Model.Email}. Please go to your mail to continue.";

        return View(request);
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ResetPassword(ResetUserPasswordRequest request)
    {
        return View(request);
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> PostResetPassword(ResetUserPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("ResetPassword",request);

        var result = await _authService.ResetPasswordAsync(request);
        if (!result.IsSuccess)
        {
            TempData[Cons.Error] = result.Message;
            return RedirectToAction("ResetPassword", request);
        }
        TempData[Cons.Success] = $"{result.Model.Message}";

        return RedirectToAction("Login");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        var claims = User.GetUserInformation();

        var result = await _authService.LogoutAsync(new LogoutUserRequest { UserId = claims.UserId});
        if (!result.IsSuccess)
        {
            TempData[Cons.Error] = result.Message;
            return RedirectToAction("Index","Home");
        }
        
        TempData[Cons.Success] = $"{result.Model.Message}";
        await _cookieAuthenticationService.SignOutAsync(HttpContext);

        return RedirectToAction("Login");
    }
}
