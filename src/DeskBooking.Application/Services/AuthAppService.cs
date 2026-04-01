
using DeskBooking.Application.Models;
using DeskBooking.Domain.Entities;
using DeskBooking.Domain.Interfaces;
using DeskBooking.Domain.Security;

namespace DeskBooking.Application.Services;

public class AuthAppService
{
    private readonly IUserRepository _userRepository;

    public AuthAppService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AppResult<User>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return AppResult<User>.Fail("Логин и пароль обязательны.");
        }

        var user = await _userRepository.GetByEmailAsync(email.Trim(), cancellationToken);
        if (user is null)
        {
            return AppResult<User>.Fail("Пользователь не найден.");
        }

        if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return AppResult<User>.Fail("Неверный пароль.");
        }

        return AppResult<User>.Ok(user, "Успешная аутентификация.");
    }
}
