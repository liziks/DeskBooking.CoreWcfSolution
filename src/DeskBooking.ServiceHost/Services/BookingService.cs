
using DeskBooking.Application.Mapping;
using DeskBooking.Application.Services;
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Contracts.ServiceContracts;
using DeskBooking.Domain.Interfaces;
using DeskBooking.ServiceHost.Security;

namespace DeskBooking.ServiceHost.Services;

public class BookingService : AuthenticatedServiceBase, IBookingService
{
    private readonly BookingAppService _bookingAppService;
    private readonly RoomAppService _roomAppService;

    public BookingService(
        BookingAppService bookingAppService,
        RoomAppService roomAppService,
        IUserRepository userRepository,
        ISessionManager sessionManager)
        : base(userRepository, sessionManager)
    {
        _bookingAppService = bookingAppService;
        _roomAppService = roomAppService;
    }

    public async Task<BookingListResponseDto> GetAllBookingsAsync(string sessionToken, BookingFilterDto filter)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return new BookingListResponseDto { Success = false, Message = auth.Message };
        }

        var bookings = await _bookingAppService.GetAllAsync(filter, auth.User);

        return new BookingListResponseDto
        {
            Success = true,
            Message = "OK",
            Bookings = bookings.Select(x => x.ToDto()).ToList()
        };
    }

    public async Task<BookingResponseDto> GetBookingByIdAsync(string sessionToken, int bookingId)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return new BookingResponseDto { Success = false, Message = auth.Message };
        }

        var result = await _bookingAppService.GetByIdAsync(bookingId, auth.User);

        return new BookingResponseDto
        {
            Success = result.Success,
            Message = result.Message,
            Booking = result.Value?.ToDto()
        };
    }

    public async Task<OperationResultDto> CreateBookingAsync(string sessionToken, CreateBookingRequestDto request)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return Unauthorized(auth.Message);
        }

        var result = await _bookingAppService.CreateAsync(request, auth.User);
        return new OperationResultDto
        {
            Success = result.Success,
            Message = result.Message,
            EntityId = result.EntityId
        };
    }

    public async Task<OperationResultDto> UpdateBookingAsync(string sessionToken, UpdateBookingRequestDto request)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return Unauthorized(auth.Message);
        }

        var result = await _bookingAppService.UpdateAsync(request, auth.User);
        return new OperationResultDto
        {
            Success = result.Success,
            Message = result.Message,
            EntityId = result.EntityId
        };
    }

    public async Task<OperationResultDto> CancelBookingAsync(string sessionToken, int bookingId, string? reason)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return Unauthorized(auth.Message);
        }

        var result = await _bookingAppService.CancelAsync(bookingId, reason, auth.User);
        return new OperationResultDto
        {
            Success = result.Success,
            Message = result.Message,
            EntityId = result.EntityId
        };
    }

    public async Task<RoomListResponseDto> GetAvailableRoomsAsync(string sessionToken, AvailabilityRequestDto request)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return new RoomListResponseDto { Success = false, Message = auth.Message };
        }

        var rooms = await _roomAppService.GetAvailableAsync(request);

        return new RoomListResponseDto
        {
            Success = true,
            Message = "OK",
            Rooms = rooms.Select(x => x.ToDto()).ToList()
        };
    }
}
