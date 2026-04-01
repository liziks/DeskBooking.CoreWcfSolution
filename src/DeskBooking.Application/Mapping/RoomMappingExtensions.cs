
using DeskBooking.Contracts.DataContracts;
using DeskBooking.Domain.Entities;

namespace DeskBooking.Application.Mapping;

public static class RoomMappingExtensions
{
    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Location = room.Location,
            Capacity = room.Capacity,
            HasProjector = room.HasProjector,
            HasWhiteboard = room.HasWhiteboard,
            Description = room.Description
        };
    }
}
