
using DeskBooking.Contracts.DataContracts;

namespace DeskBooking.WebClient.ViewModels;

public class AvailabilitySearchViewModel
{
    public DateTime? StartLocal { get; set; }
    public DateTime? EndLocal { get; set; }
    public int? MinCapacity { get; set; }
    public bool RequiresProjector { get; set; }
    public bool RequiresWhiteboard { get; set; }
    public List<RoomDto> AvailableRooms { get; set; } = new();
}
