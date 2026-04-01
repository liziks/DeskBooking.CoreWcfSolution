
namespace DeskBooking.Application.Models;

public class AppResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public int? EntityId { get; init; }

    public static AppResult Ok(string message = "OK", int? entityId = null) =>
        new() { Success = true, Message = message, EntityId = entityId };

    public static AppResult Fail(string message) =>
        new() { Success = false, Message = message };
}

public class AppResult<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Value { get; init; }

    public static AppResult<T> Ok(T value, string message = "OK") =>
        new() { Success = true, Message = message, Value = value };

    public static AppResult<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
