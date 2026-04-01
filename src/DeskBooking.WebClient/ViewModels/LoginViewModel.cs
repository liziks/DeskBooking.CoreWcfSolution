
using System.ComponentModel.DataAnnotations;

namespace DeskBooking.WebClient.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email обязателен.")]
    [EmailAddress(ErrorMessage = "Неверный формат email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}
