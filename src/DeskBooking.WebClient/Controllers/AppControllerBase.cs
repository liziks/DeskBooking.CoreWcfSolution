
using Microsoft.AspNetCore.Mvc;
using DeskBooking.Contracts.Enums;
using DeskBooking.WebClient.Infrastructure;

namespace DeskBooking.WebClient.Controllers;

public abstract class AppControllerBase : Controller
{
    protected string? SessionToken => HttpContext.Session.GetString(SessionKeys.SessionToken);

    protected int CurrentUserId =>
        int.TryParse(HttpContext.Session.GetString(SessionKeys.UserId), out var userId)
            ? userId
            : 0;

    protected bool IsAuthenticated => !string.IsNullOrWhiteSpace(SessionToken);

    protected bool IsAdmin => string.Equals(
        HttpContext.Session.GetString(SessionKeys.UserRole),
        UserRole.Admin.ToString(),
        StringComparison.OrdinalIgnoreCase);

    protected IActionResult RedirectToLogin() => RedirectToAction("Login", "Auth");

    protected void SetSuccess(string message) => TempData["SuccessMessage"] = message;
    protected void SetError(string message) => TempData["ErrorMessage"] = message;
}
