
using DeskBooking.Contracts.Enums;

namespace DeskBooking.ServiceHost.Security;

public class SessionInfo
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}
