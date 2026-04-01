
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Contracts.ServiceContracts;

namespace DeskBooking.WebClient.Services;

public class BookingApiClient
{
    private readonly SoapClientExecutor _executor;

    public BookingApiClient(SoapClientExecutor executor)
    {
        _executor = executor;
    }

    public Task<BookingListResponseDto> GetAllAsync(string sessionToken, BookingFilterDto filter)
    {
        return _executor.ExecuteAsync<IBookingService, BookingListResponseDto>(
            _executor.BookingServiceUrl,
            client => client.GetAllBookingsAsync(sessionToken, filter));
    }

    public Task<BookingResponseDto> GetByIdAsync(string sessionToken, int bookingId)
    {
        return _executor.ExecuteAsync<IBookingService, BookingResponseDto>(
            _executor.BookingServiceUrl,
            client => client.GetBookingByIdAsync(sessionToken, bookingId));
    }

    public Task<OperationResultDto> CreateAsync(string sessionToken, CreateBookingRequestDto request)
    {
        return _executor.ExecuteAsync<IBookingService, OperationResultDto>(
            _executor.BookingServiceUrl,
            client => client.CreateBookingAsync(sessionToken, request));
    }

    public Task<OperationResultDto> UpdateAsync(string sessionToken, UpdateBookingRequestDto request)
    {
        return _executor.ExecuteAsync<IBookingService, OperationResultDto>(
            _executor.BookingServiceUrl,
            client => client.UpdateBookingAsync(sessionToken, request));
    }

    public Task<OperationResultDto> CancelAsync(string sessionToken, int bookingId, string? reason)
    {
        return _executor.ExecuteAsync<IBookingService, OperationResultDto>(
            _executor.BookingServiceUrl,
            client => client.CancelBookingAsync(sessionToken, bookingId, reason));
    }

    public Task<RoomListResponseDto> GetAvailableRoomsAsync(string sessionToken, AvailabilityRequestDto request)
    {
        return _executor.ExecuteAsync<IBookingService, RoomListResponseDto>(
            _executor.BookingServiceUrl,
            client => client.GetAvailableRoomsAsync(sessionToken, request));
    }
}
