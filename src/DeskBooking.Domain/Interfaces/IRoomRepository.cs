
using DeskBooking.Domain.Entities;

namespace DeskBooking.Domain.Interfaces;

public interface IRoomRepository
{
    Task<List<Room>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Room?> GetByIdAsync(int roomId, CancellationToken cancellationToken = default);
    Task<List<Room>> GetAvailableAsync(
        DateTime startUtc,
        DateTime endUtc,
        int? minCapacity,
        bool requiresProjector,
        bool requiresWhiteboard,
        CancellationToken cancellationToken = default);
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
    void Remove(Room room);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
