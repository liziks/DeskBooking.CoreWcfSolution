
using DeskBooking.Application.Mapping;
using DeskBooking.Application.Services;
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Contracts.ServiceContracts;
using DeskBooking.Domain.Interfaces;
using DeskBooking.ServiceHost.Security;

namespace DeskBooking.ServiceHost.Services;

public class RoomService : AuthenticatedServiceBase, IRoomService
{
    private readonly RoomAppService _roomAppService;

    public RoomService(
        RoomAppService roomAppService,
        IUserRepository userRepository,
        ISessionManager sessionManager)
        : base(userRepository, sessionManager)
    {
        _roomAppService = roomAppService;
    }

    public async Task<RoomListResponseDto> GetAllRoomsAsync(string sessionToken, RoomFilterDto filter)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return new RoomListResponseDto { Success = false, Message = auth.Message };
        }

        var rooms = await _roomAppService.GetAllAsync(filter, CancellationToken.None);

        return new RoomListResponseDto
        {
            Success = true,
            Message = "OK",
            Rooms = rooms.Select(x => x.ToDto()).ToList()
        };
    }

    public async Task<RoomResponseDto> GetRoomByIdAsync(string sessionToken, int roomId)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return new RoomResponseDto { Success = false, Message = auth.Message };
        }

        var result = await _roomAppService.GetByIdAsync(roomId);
        return new RoomResponseDto
        {
            Success = result.Success,
            Message = result.Message,
            Room = result.Value?.ToDto()
        };
    }

    public async Task<OperationResultDto> CreateRoomAsync(string sessionToken, RoomDto room)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return Unauthorized(auth.Message);
        }

        var result = await _roomAppService.CreateAsync(room, auth.User);
        return new OperationResultDto
        {
            Success = result.Success,
            Message = result.Message,
            EntityId = result.EntityId
        };
    }

    public async Task<OperationResultDto> UpdateRoomAsync(string sessionToken, RoomDto room)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return Unauthorized(auth.Message);
        }

        var result = await _roomAppService.UpdateAsync(room, auth.User);
        return new OperationResultDto
        {
            Success = result.Success,
            Message = result.Message,
            EntityId = result.EntityId
        };
    }

    public async Task<OperationResultDto> DeleteRoomAsync(string sessionToken, int roomId)
    {
        var auth = await TryGetCurrentUserAsync(sessionToken);
        if (!auth.Success || auth.User is null)
        {
            return Unauthorized(auth.Message);
        }

        var result = await _roomAppService.DeleteAsync(roomId, auth.User);
        return new OperationResultDto
        {
            Success = result.Success,
            Message = result.Message,
            EntityId = result.EntityId
        };
    }
}
