
using DeskBooking.Domain.Entities;

namespace DeskBooking.ServiceHost.Security;

public interface ISessionManager
{
    string CreateSession(User user);
    SessionInfo? GetSession(string token);
    void RemoveSession(string token);
}
