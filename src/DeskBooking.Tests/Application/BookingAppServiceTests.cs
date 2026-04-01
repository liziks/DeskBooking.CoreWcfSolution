
using DeskBooking.Application.Services;
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Contracts.Enums;
using DeskBooking.Domain.Entities;
using DeskBooking.Domain.Interfaces;
using Xunit;

namespace DeskBooking.Tests.Application;

public class BookingAppServiceTests
{
    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenRoomHasOverlap()
    {
        var roomRepository = new FakeRoomRepository(new Room
        {
            Id = 10,
            Name = "Alpha",
            Capacity = 5
        });

        var bookingRepository = new FakeBookingRepository(hasOverlap: true);
        var service = new BookingAppService(bookingRepository, roomRepository);

        var currentUser = new User
        {
            Id = 5,
            DisplayName = "Employee",
            Role = UserRole.Employee
        };

        var result = await service.CreateAsync(new CreateBookingRequestDto
        {
            RoomId = 10,
            Title = "Daily sync",
            Purpose = "Обсуждение задач",
            StartUtc = DateTime.UtcNow.AddHours(2),
            EndUtc = DateTime.UtcNow.AddHours(3),
            ParticipantCount = 4
        }, currentUser);

        Assert.False(result.Success);
        Assert.Equal("Комната уже занята в выбранный интервал.", result.Message);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenParticipantCountExceedsRoomCapacity()
    {
        var roomRepository = new FakeRoomRepository(new Room
        {
            Id = 10,
            Name = "Alpha",
            Capacity = 3
        });

        var bookingRepository = new FakeBookingRepository(hasOverlap: false);
        var service = new BookingAppService(bookingRepository, roomRepository);

        var currentUser = new User
        {
            Id = 5,
            DisplayName = "Employee",
            Role = UserRole.Employee
        };

        var result = await service.CreateAsync(new CreateBookingRequestDto
        {
            RoomId = 10,
            Title = "Daily sync",
            Purpose = "Обсуждение задач",
            StartUtc = DateTime.UtcNow.AddHours(2),
            EndUtc = DateTime.UtcNow.AddHours(3),
            ParticipantCount = 5
        }, currentUser);

        Assert.False(result.Success);
        Assert.Equal("Количество участников превышает вместимость комнаты.", result.Message);
    }

    [Fact]
    public async Task CancelAsync_ReturnsFailure_WhenUserDoesNotOwnBooking_AndIsNotAdmin()
    {
        var roomRepository = new FakeRoomRepository(new Room
        {
            Id = 10,
            Name = "Alpha",
            Capacity = 5
        });

        var booking = new Booking
        {
            Id = 7,
            RoomId = 10,
            UserId = 99,
            Title = "Secret",
            StartUtc = DateTime.UtcNow.AddHours(2),
            EndUtc = DateTime.UtcNow.AddHours(3),
            ParticipantCount = 2,
            Status = BookingStatus.Active
        };

        var bookingRepository = new FakeBookingRepository(hasOverlap: false, booking: booking);
        var service = new BookingAppService(bookingRepository, roomRepository);

        var currentUser = new User
        {
            Id = 5,
            DisplayName = "Employee",
            Role = UserRole.Employee
        };

        var result = await service.CancelAsync(7, null, currentUser);

        Assert.False(result.Success);
        Assert.Equal("Отменить бронирование может только владелец или администратор.", result.Message);
    }

    private sealed class FakeRoomRepository : IRoomRepository
    {
        private readonly Room? _room;

        public FakeRoomRepository(Room? room)
        {
            _room = room;
        }

        public Task<List<Room>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_room is null ? new List<Room>() : new List<Room> { _room });
        }

        public Task<Room?> GetByIdAsync(int roomId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_room?.Id == roomId ? _room : null);
        }

        public Task<List<Room>> GetAvailableAsync(DateTime startUtc, DateTime endUtc, int? minCapacity, bool requiresProjector, bool requiresWhiteboard, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_room is null ? new List<Room>() : new List<Room> { _room });
        }

        public Task AddAsync(Room room, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(Room room) { }
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeBookingRepository : IBookingRepository
    {
        private readonly bool _hasOverlap;
        private readonly Booking? _booking;

        public FakeBookingRepository(bool hasOverlap, Booking? booking = null)
        {
            _hasOverlap = hasOverlap;
            _booking = booking;
        }

        public Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_booking is null ? new List<Booking>() : new List<Booking> { _booking });
        }

        public Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_booking?.Id == bookingId ? _booking : null);
        }

        public Task<bool> HasOverlapAsync(int roomId, DateTime startUtc, DateTime endUtc, int? excludeBookingId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_hasOverlap);
        }

        public Task<bool> HasActiveFutureBookingsForRoomAsync(int roomId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task AddAsync(Booking booking, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
