
using Microsoft.AspNetCore.Mvc;
using DeskBooking.Contracts.DataContracts;
using DeskBooking.WebClient.Infrastructure;
using DeskBooking.WebClient.Services;
using DeskBooking.WebClient.ViewModels;

namespace DeskBooking.WebClient.Controllers;

public class AuthController : AppControllerBase
{
    private readonly AuthApiClient _authApiClient;

    public AuthController(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (IsAuthenticated)
        {
            return RedirectToAction("Index", "Rooms");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _authApiClient.LoginAsync(new LoginRequestDto
            {
                Email = model.Email,
                Password = model.Password
            });

            if (!response.Success)
            {
                model.ErrorMessage = response.Message;
                return View(model);
            }

            HttpContext.Session.SetString(SessionKeys.SessionToken, response.SessionToken);
            HttpContext.Session.SetString(SessionKeys.DisplayName, response.DisplayName);
            HttpContext.Session.SetString(SessionKeys.UserRole, response.Role.ToString());
            HttpContext.Session.SetString(SessionKeys.UserId, response.UserId.ToString());

            return RedirectToAction("Index", "Rooms");
        }
        catch (Exception ex)
        {
            model.ErrorMessage = $"Ошибка подключения к сервису: {ex.Message}";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var token = SessionToken;
        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                await _authApiClient.LogoutAsync(token);
            }
            catch
            {
            }
        }

        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
