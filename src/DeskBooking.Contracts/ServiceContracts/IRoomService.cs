
using System.ServiceModel;
using DeskBooking.Contracts.DataContracts;

namespace DeskBooking.Contracts.ServiceContracts;

[ServiceContract(Name = "RoomService", Namespace = ContractNamespaces.Services)]
public interface IRoomService
{
    [OperationContract]
    Task<RoomListResponseDto> GetAllRoomsAsync(string sessionToken, RoomFilterDto filter);

    [OperationContract]
    Task<RoomResponseDto> GetRoomByIdAsync(string sessionToken, int roomId);

    [OperationContract]
    Task<OperationResultDto> CreateRoomAsync(string sessionToken, RoomDto room);

    [OperationContract]
    Task<OperationResultDto> UpdateRoomAsync(string sessionToken, RoomDto room);

    [OperationContract]
    Task<OperationResultDto> DeleteRoomAsync(string sessionToken, int roomId);
}
