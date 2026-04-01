
using System.Runtime.Serialization;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class BookingFilterDto
{
    [DataMember(Order = 1, EmitDefaultValue = false)]
    public DateTime? FromUtc { get; set; }

    [DataMember(Order = 2, EmitDefaultValue = false)]
    public DateTime? ToUtc { get; set; }

    [DataMember(Order = 3, EmitDefaultValue = false)]
    public int? RoomId { get; set; }

    [DataMember(Order = 4)]
    public bool OnlyMyBookings { get; set; }
}
