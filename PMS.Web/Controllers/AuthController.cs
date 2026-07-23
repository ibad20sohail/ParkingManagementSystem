using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.Application.Constants;
using PMS.Application.IServices;
using PMS.Application.Models.Requests;

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
    public async Task<IActionResult> ForgetPassword(LoginUserRequest request)
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
}
