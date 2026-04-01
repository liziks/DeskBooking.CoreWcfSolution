
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Contracts.ServiceContracts;

namespace DeskBooking.WebClient.Services;

public class RoomApiClient
{
    private readonly SoapClientExecutor _executor;

    public RoomApiClient(SoapClientExecutor executor)
    {
        _executor = executor;
    }

    public Task<RoomListResponseDto> GetAllAsync(string sessionToken, RoomFilterDto filter)
    {
        return _executor.ExecuteAsync<IRoomService, RoomListResponseDto>(
            _executor.RoomServiceUrl,
            client => client.GetAllRoomsAsync(sessionToken, filter));
    }

    public Task<RoomResponseDto> GetByIdAsync(string sessionToken, int roomId)
    {
        return _executor.ExecuteAsync<IRoomService, RoomResponseDto>(
            _executor.RoomServiceUrl,
            client => client.GetRoomByIdAsync(sessionToken, roomId));
    }

    public Task<OperationResultDto> CreateAsync(string sessionToken, RoomDto room)
    {
        return _executor.ExecuteAsync<IRoomService, OperationResultDto>(
            _executor.RoomServiceUrl,
            client => client.CreateRoomAsync(sessionToken, room));
    }

    public Task<OperationResultDto> UpdateAsync(string sessionToken, RoomDto room)
    {
        return _executor.ExecuteAsync<IRoomService, OperationResultDto>(
            _executor.RoomServiceUrl,
            client => client.UpdateRoomAsync(sessionToken, room));
    }

    public Task<OperationResultDto> DeleteAsync(string sessionToken, int roomId)
    {
        return _executor.ExecuteAsync<IRoomService, OperationResultDto>(
            _executor.RoomServiceUrl,
            client => client.DeleteRoomAsync(sessionToken, roomId));
    }
}
