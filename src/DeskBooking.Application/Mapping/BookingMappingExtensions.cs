
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Domain.Entities;

namespace DeskBooking.Application.Mapping;

public static class BookingMappingExtensions
{
    public static BookingDto ToDto(this Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomName = booking.Room?.Name ?? string.Empty,
            UserId = booking.UserId,
            UserDisplayName = booking.User?.DisplayName ?? string.Empty,
            Title = booking.Title,
            Purpose = booking.Purpose,
            StartUtc = booking.StartUtc,
            EndUtc = booking.EndUtc,
            ParticipantCount = booking.ParticipantCount,
            Status = booking.Status,
            CreatedAtUtc = booking.CreatedAtUtc,
            UpdatedAtUtc = booking.UpdatedAtUtc,
            CancelReason = booking.CancelReason
        };
    }
}
