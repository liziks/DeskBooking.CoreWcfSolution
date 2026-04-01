
using Microsoft.EntityFrameworkCore;
using DeskBooking.Contracts.Enums;
using DeskBooking.Domain.Entities;
using DeskBooking.Domain.Interfaces;
using DeskBooking.Infrastructure.Persistence;

namespace DeskBooking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _dbContext;

    public BookingRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Bookings
            .Include(x => x.Room)
            .Include(x => x.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Bookings
            .Include(x => x.Room)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == bookingId, cancellationToken);
    }

    public Task<bool> HasOverlapAsync(
        int roomId,
        DateTime startUtc,
        DateTime endUtc,
        int? excludeBookingId = null,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Bookings.AnyAsync(x =>
            x.RoomId == roomId &&
            x.Status == BookingStatus.Active &&
            (!excludeBookingId.HasValue || x.Id != excludeBookingId.Value) &&
            x.StartUtc < endUtc &&
            x.EndUtc > startUtc,
            cancellationToken);
    }

    public Task<bool> HasActiveFutureBookingsForRoomAsync(int roomId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Bookings.AnyAsync(x =>
            x.RoomId == roomId &&
            x.Status == BookingStatus.Active &&
            x.EndUtc > DateTime.UtcNow,
            cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _dbContext.Bookings.AddAsync(booking, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
