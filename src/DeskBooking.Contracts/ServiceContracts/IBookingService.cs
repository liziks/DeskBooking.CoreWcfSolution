
using System.ServiceModel;
using DeskBooking.Contracts.DataContracts;

namespace DeskBooking.Contracts.ServiceContracts;

[ServiceContract(Name = "BookingService", Namespace = ContractNamespaces.Services)]
public interface IBookingService
{
    [OperationContract]
    Task<BookingListResponseDto> GetAllBookingsAsync(string sessionToken, BookingFilterDto filter);

    [OperationContract]
    Task<BookingResponseDto> GetBookingByIdAsync(string sessionToken, int bookingId);

    [OperationContract]
    Task<OperationResultDto> CreateBookingAsync(string sessionToken, CreateBookingRequestDto request);

    [OperationContract]
    Task<OperationResultDto> UpdateBookingAsync(string sessionToken, UpdateBookingRequestDto request);

    [OperationContract]
    Task<OperationResultDto> CancelBookingAsync(string sessionToken, int bookingId, string? reason);

    [OperationContract]
    Task<RoomListResponseDto> GetAvailableRoomsAsync(string sessionToken, AvailabilityRequestDto request);
}
