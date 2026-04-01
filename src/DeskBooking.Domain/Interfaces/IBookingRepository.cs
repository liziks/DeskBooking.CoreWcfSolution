
using DeskBooking.Domain.Entities;

namespace DeskBooking.Domain.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(
        int roomId,
        DateTime startUtc,
        DateTime endUtc,
        int? excludeBookingId = null,
        CancellationToken cancellationToken = default);
    Task<bool> HasActiveFutureBookingsForRoomAsync(int roomId, CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
