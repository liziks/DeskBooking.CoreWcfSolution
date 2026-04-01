
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Domain.Entities;
using DeskBooking.Domain.Interfaces;
using DeskBooking.ServiceHost.Security;

namespace DeskBooking.ServiceHost.Services;

public abstract class AuthenticatedServiceBase
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionManager _sessionManager;

    protected AuthenticatedServiceBase(IUserRepository userRepository, ISessionManager sessionManager)
    {
        _userRepository = userRepository;
        _sessionManager = sessionManager;
    }

    protected async Task<(bool Success, string Message, User? User)> TryGetCurrentUserAsync(
        string sessionToken,
        CancellationToken cancellationToken = default)
    {
        var session = _sessionManager.GetSession(sessionToken);
        if (session is null)
        {
            return (false, "Сессия не найдена или истекла.", null);
        }

        var user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            _sessionManager.RemoveSession(sessionToken);
            return (false, "Пользователь не найден.", null);
        }

        return (true, string.Empty, user);
    }

    protected static OperationResultDto Unauthorized(string message) =>
        new()
        {
            Success = false,
            Message = message
        };
}
