
using System.Runtime.Serialization;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class CreateBookingRequestDto
{
    [DataMember(Order = 1)]
    public int RoomId { get; set; }

    [DataMember(Order = 2)]
    public string Title { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string Purpose { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public DateTime StartUtc { get; set; }

    [DataMember(Order = 5)]
    public DateTime EndUtc { get; set; }

    [DataMember(Order = 6)]
    public int ParticipantCount { get; set; }
}
