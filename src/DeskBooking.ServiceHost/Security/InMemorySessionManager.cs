
using System.Collections.Concurrent;
using DeskBooking.Domain.Entities;

namespace DeskBooking.ServiceHost.Security;

public class InMemorySessionManager : ISessionManager
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(8);
    private readonly ConcurrentDictionary<string, SessionInfo> _sessions = new();

    public string CreateSession(User user)
    {
        var token = Guid.NewGuid().ToString("N");
        var session = new SessionInfo
        {
            Token = token,
            UserId = user.Id,
            DisplayName = user.DisplayName,
            Role = user.Role,
            ExpiresAtUtc = DateTime.UtcNow.Add(SessionLifetime)
        };

        _sessions[token] = session;
        return token;
    }

    public SessionInfo? GetSession(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        if (!_sessions.TryGetValue(token, out var session))
        {
            return null;
        }

        if (session.ExpiresAtUtc <= DateTime.UtcNow)
        {
            _sessions.TryRemove(token, out _);
            return null;
        }

        return session;
    }

    public void RemoveSession(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        _sessions.TryRemove(token, out _);
    }
}
