
using DeskBooking.Application.Models;
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Contracts.Enums;
using DeskBooking.Domain.Entities;
using DeskBooking.Domain.Interfaces;

namespace DeskBooking.Application.Services;

public class RoomAppService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public RoomAppService(IRoomRepository roomRepository, IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<List<Room>> GetAllAsync(RoomFilterDto? filter, CancellationToken cancellationToken = default)
    {
        var rooms = await _roomRepository.GetAllAsync(cancellationToken);

        if (filter is null)
        {
            return rooms;
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            rooms = rooms.Where(x =>
                    x.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Location.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Description.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (filter.MinCapacity.HasValue)
        {
            rooms = rooms.Where(x => x.Capacity >= filter.MinCapacity.Value).ToList();
        }

        if (filter.RequiresProjector)
        {
            rooms = rooms.Where(x => x.HasProjector).ToList();
        }

        if (filter.RequiresWhiteboard)
        {
            rooms = rooms.Where(x => x.HasWhiteboard).ToList();
        }

        return rooms.OrderBy(x => x.Name).ToList();
    }

    public async Task<AppResult<Room>> GetByIdAsync(int roomId, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);
        return room is null
            ? AppResult<Room>.Fail("Комната не найдена.")
            : AppResult<Room>.Ok(room);
    }

    public async Task<AppResult> CreateAsync(RoomDto roomDto, User currentUser, CancellationToken cancellationToken = default)
    {
        if (currentUser.Role != UserRole.Admin)
        {
            return AppResult.Fail("Только администратор может создавать комнаты.");
        }

        if (string.IsNullOrWhiteSpace(roomDto.Name))
        {
            return AppResult.Fail("Название комнаты обязательно.");
        }

        if (roomDto.Capacity <= 0)
        {
            return AppResult.Fail("Вместимость комнаты должна быть больше нуля.");
        }

        var room = new Room
        {
            Name = roomDto.Name.Trim(),
            Location = roomDto.Location.Trim(),
            Capacity = roomDto.Capacity,
            HasProjector = roomDto.HasProjector,
            HasWhiteboard = roomDto.HasWhiteboard,
            Description = roomDto.Description.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _roomRepository.AddAsync(room, cancellationToken);
        await _roomRepository.SaveChangesAsync(cancellationToken);

        return AppResult.Ok("Комната успешно создана.", room.Id);
    }

    public async Task<AppResult> UpdateAsync(RoomDto roomDto, User currentUser, CancellationToken cancellationToken = default)
    {
        if (currentUser.Role != UserRole.Admin)
        {
            return AppResult.Fail("Только администратор может редактировать комнаты.");
        }

        if (string.IsNullOrWhiteSpace(roomDto.Name))
        {
            return AppResult.Fail("Название комнаты обязательно.");
        }

        if (roomDto.Capacity <= 0)
        {
            return AppResult.Fail("Вместимость комнаты должна быть больше нуля.");
        }

        var room = await _roomRepository.GetByIdAsync(roomDto.Id, cancellationToken);
        if (room is null)
        {
            return AppResult.Fail("Комната не найдена.");
        }

        room.Name = roomDto.Name.Trim();
        room.Location = roomDto.Location.Trim();
        room.Capacity = roomDto.Capacity;
        room.HasProjector = roomDto.HasProjector;
        room.HasWhiteboard = roomDto.HasWhiteboard;
        room.Description = roomDto.Description.Trim();

        await _roomRepository.SaveChangesAsync(cancellationToken);
        return AppResult.Ok("Комната успешно обновлена.", room.Id);
    }

    public async Task<AppResult> DeleteAsync(int roomId, User currentUser, CancellationToken cancellationToken = default)
    {
        if (currentUser.Role != UserRole.Admin)
        {
            return AppResult.Fail("Только администратор может удалять комнаты.");
        }

        var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            return AppResult.Fail("Комната не найдена.");
        }

        var hasActiveBookings = await _bookingRepository.HasActiveFutureBookingsForRoomAsync(roomId, cancellationToken);
        if (hasActiveBookings)
        {
            return AppResult.Fail("Нельзя удалить комнату, у которой есть активные будущие бронирования.");
        }

        _roomRepository.Remove(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);

        return AppResult.Ok("Комната успешно удалена.", roomId);
    }

    public async Task<List<Room>> GetAvailableAsync(AvailabilityRequestDto request, CancellationToken cancellationToken = default)
    {
        return await _roomRepository.GetAvailableAsync(
            request.StartUtc,
            request.EndUtc,
            request.MinCapacity,
            request.RequiresProjector,
            request.RequiresWhiteboard,
            cancellationToken);
    }
}
