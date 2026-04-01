
using Microsoft.EntityFrameworkCore;
using DeskBooking.Contracts.Enums;
using DeskBooking.Domain.Entities;
using DeskBooking.Domain.Interfaces;
using DeskBooking.Infrastructure.Persistence;

namespace DeskBooking.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _dbContext;

    public RoomRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Room>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Rooms
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Room?> GetByIdAsync(int roomId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Rooms
            .FirstOrDefaultAsync(x => x.Id == roomId, cancellationToken);
    }

    public async Task<List<Room>> GetAvailableAsync(
        DateTime startUtc,
        DateTime endUtc,
        int? minCapacity,
        bool requiresProjector,
        bool requiresWhiteboard,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Rooms.AsQueryable();

        if (minCapacity.HasValue)
        {
            query = query.Where(x => x.Capacity >= minCapacity.Value);
        }

        if (requiresProjector)
        {
            query = query.Where(x => x.HasProjector);
        }

        if (requiresWhiteboard)
        {
            query = query.Where(x => x.HasWhiteboard);
        }

        query = query.Where(room =>
            !_dbContext.Bookings.Any(booking =>
                booking.RoomId == room.Id &&
                booking.Status == BookingStatus.Active &&
                booking.StartUtc < endUtc &&
                booking.EndUtc > startUtc));

        return await query
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        await _dbContext.Rooms.AddAsync(room, cancellationToken);
    }

    public void Remove(Room room)
    {
        _dbContext.Rooms.Remove(room);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
