
namespace DeskBooking.Application.Validators;

public static class BookingValidator
{
    public static string? ValidateTimeRange(DateTime startUtc, DateTime endUtc)
    {
        if (startUtc == default || endUtc == default)
        {
            return "Дата и время бронирования должны быть заполнены.";
        }

        if (startUtc >= endUtc)
        {
            return "Время начала должно быть раньше времени окончания.";
        }

        if (startUtc < DateTime.UtcNow)
        {
            return "Нельзя создавать бронирование в прошлом.";
        }

        return null;
    }

    public static string? ValidateParticipantCount(int participantCount)
    {
        if (participantCount <= 0)
        {
            return "Количество участников должно быть больше нуля.";
        }

        return null;
    }
}
