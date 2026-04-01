
using DeskBooking.Application.Services;
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Contracts.ServiceContracts;
using DeskBooking.ServiceHost.Security;

namespace DeskBooking.ServiceHost.Services;

public class AuthService : IAuthService
{
    private readonly AuthAppService _authAppService;
    private readonly ISessionManager _sessionManager;

    public AuthService(AuthAppService authAppService, ISessionManager sessionManager)
    {
        _authAppService = authAppService;
        _sessionManager = sessionManager;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var result = await _authAppService.AuthenticateAsync(request.Email, request.Password);

        if (!result.Success || result.Value is null)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = result.Message
            };
        }

        var token = _sessionManager.CreateSession(result.Value);

        return new LoginResponseDto
        {
            Success = true,
            Message = "Вход выполнен успешно.",
            SessionToken = token,
            UserId = result.Value.Id,
            DisplayName = result.Value.DisplayName,
            Role = result.Value.Role
        };
    }

    public Task<OperationResultDto> LogoutAsync(string sessionToken)
    {
        _sessionManager.RemoveSession(sessionToken);

        return Task.FromResult(new OperationResultDto
        {
            Success = true,
            Message = "Выход выполнен."
        });
    }
}
