
using DeskBooking.Application.Models;
using DeskBooking.Application.Validators;
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Contracts.Enums;
using DeskBooking.Domain.Entities;
using DeskBooking.Domain.Interfaces;

namespace DeskBooking.Application.Services;

public class BookingAppService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    public BookingAppService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public async Task<List<Booking>> GetAllAsync(BookingFilterDto? filter, User currentUser, CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetAllAsync(cancellationToken);

        if (currentUser.Role != UserRole.Admin)
        {
            bookings = bookings.Where(x => x.UserId == currentUser.Id).ToList();
        }

        if (filter is null)
        {
            return bookings.OrderByDescending(x => x.StartUtc).ToList();
        }

        if (filter.OnlyMyBookings)
        {
            bookings = bookings.Where(x => x.UserId == currentUser.Id).ToList();
        }

        if (filter.RoomId.HasValue)
        {
            bookings = bookings.Where(x => x.RoomId == filter.RoomId.Value).ToList();
        }

        if (filter.FromUtc.HasValue)
        {
            bookings = bookings.Where(x => x.StartUtc >= filter.FromUtc.Value).ToList();
        }

        if (filter.ToUtc.HasValue)
        {
            bookings = bookings.Where(x => x.EndUtc <= filter.ToUtc.Value).ToList();
        }

        return bookings.OrderByDescending(x => x.StartUtc).ToList();
    }

    public async Task<AppResult<Booking>> GetByIdAsync(int bookingId, User currentUser, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            return AppResult<Booking>.Fail("Бронирование не найдено.");
        }

        if (currentUser.Role != UserRole.Admin && booking.UserId != currentUser.Id)
        {
            return AppResult<Booking>.Fail("Доступ к бронированию запрещён.");
        }

        return AppResult<Booking>.Ok(booking);
    }

    public async Task<AppResult> CreateAsync(CreateBookingRequestDto request, User currentUser, CancellationToken cancellationToken = default)
    {
        var timeValidation = BookingValidator.ValidateTimeRange(request.StartUtc, request.EndUtc);
        if (timeValidation is not null)
        {
            return AppResult.Fail(timeValidation);
        }

        var participantValidation = BookingValidator.ValidateParticipantCount(request.ParticipantCount);
        if (participantValidation is not null)
        {
            return AppResult.Fail(participantValidation);
        }

        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null)
        {
            return AppResult.Fail("Комната не найдена.");
        }

        if (request.ParticipantCount > room.Capacity)
        {
            return AppResult.Fail("Количество участников превышает вместимость комнаты.");
        }

        var hasOverlap = await _bookingRepository.HasOverlapAsync(
            request.RoomId,
            request.StartUtc,
            request.EndUtc,
            null,
            cancellationToken);

        if (hasOverlap)
        {
            return AppResult.Fail("Комната уже занята в выбранный интервал.");
        }

        var booking = new Booking
        {
            RoomId = request.RoomId,
            UserId = currentUser.Id,
            Title = request.Title.Trim(),
            Purpose = request.Purpose.Trim(),
            StartUtc = request.StartUtc,
            EndUtc = request.EndUtc,
            ParticipantCount = request.ParticipantCount,
            Status = BookingStatus.Active,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        return AppResult.Ok("Бронирование успешно создано.", booking.Id);
    }

    public async Task<AppResult> UpdateAsync(UpdateBookingRequestDto request, User currentUser, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
        {
            return AppResult.Fail("Бронирование не найдено.");
        }

        if (currentUser.Role != UserRole.Admin && booking.UserId != currentUser.Id)
        {
            return AppResult.Fail("Редактировать бронирование может только владелец или администратор.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return AppResult.Fail("Нельзя редактировать отменённое бронирование.");
        }

        var timeValidation = BookingValidator.ValidateTimeRange(request.StartUtc, request.EndUtc);
        if (timeValidation is not null)
        {
            return AppResult.Fail(timeValidation);
        }

        var participantValidation = BookingValidator.ValidateParticipantCount(request.ParticipantCount);
        if (participantValidation is not null)
        {
            return AppResult.Fail(participantValidation);
        }

        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null)
        {
            return AppResult.Fail("Комната не найдена.");
        }

        if (request.ParticipantCount > room.Capacity)
        {
            return AppResult.Fail("Количество участников превышает вместимость комнаты.");
        }

        var hasOverlap = await _bookingRepository.HasOverlapAsync(
            request.RoomId,
            request.StartUtc,
            request.EndUtc,
            request.BookingId,
            cancellationToken);

        if (hasOverlap)
        {
            return AppResult.Fail("Комната уже занята в выбранный интервал.");
        }

        booking.RoomId = request.RoomId;
        booking.Title = request.Title.Trim();
        booking.Purpose = request.Purpose.Trim();
        booking.StartUtc = request.StartUtc;
        booking.EndUtc = request.EndUtc;
        booking.ParticipantCount = request.ParticipantCount;
        booking.UpdatedAtUtc = DateTime.UtcNow;

        await _bookingRepository.SaveChangesAsync(cancellationToken);
        return AppResult.Ok("Бронирование успешно обновлено.", booking.Id);
    }

    public async Task<AppResult> CancelAsync(int bookingId, string? reason, User currentUser, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            return AppResult.Fail("Бронирование не найдено.");
        }

        if (currentUser.Role != UserRole.Admin && booking.UserId != currentUser.Id)
        {
            return AppResult.Fail("Отменить бронирование может только владелец или администратор.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            return AppResult.Fail("Бронирование уже отменено.");
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancelReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        booking.UpdatedAtUtc = DateTime.UtcNow;

        await _bookingRepository.SaveChangesAsync(cancellationToken);
        return AppResult.Ok("Бронирование отменено.", booking.Id);
    }
}
