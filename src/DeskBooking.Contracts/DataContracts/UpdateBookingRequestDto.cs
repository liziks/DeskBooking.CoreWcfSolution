
using System.Runtime.Serialization;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class UpdateBookingRequestDto
{
    [DataMember(Order = 1)]
    public int BookingId { get; set; }

    [DataMember(Order = 2)]
    public int RoomId { get; set; }

    [DataMember(Order = 3)]
    public string Title { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public string Purpose { get; set; } = string.Empty;

    [DataMember(Order = 5)]
    public DateTime StartUtc { get; set; }

    [DataMember(Order = 6)]
    public DateTime EndUtc { get; set; }

    [DataMember(Order = 7)]
    public int ParticipantCount { get; set; }
}
