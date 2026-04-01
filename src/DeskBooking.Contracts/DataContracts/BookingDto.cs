
using System.Runtime.Serialization;
using DeskBooking.Contracts.Enums;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class BookingDto
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public int RoomId { get; set; }

    [DataMember(Order = 3)]
    public string RoomName { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public int UserId { get; set; }

    [DataMember(Order = 5)]
    public string UserDisplayName { get; set; } = string.Empty;

    [DataMember(Order = 6)]
    public string Title { get; set; } = string.Empty;

    [DataMember(Order = 7)]
    public string Purpose { get; set; } = string.Empty;

    [DataMember(Order = 8)]
    public DateTime StartUtc { get; set; }

    [DataMember(Order = 9)]
    public DateTime EndUtc { get; set; }

    [DataMember(Order = 10)]
    public int ParticipantCount { get; set; }

    [DataMember(Order = 11)]
    public BookingStatus Status { get; set; }

    [DataMember(Order = 12)]
    public DateTime CreatedAtUtc { get; set; }

    [DataMember(Order = 13)]
    public DateTime UpdatedAtUtc { get; set; }

    [DataMember(Order = 14, EmitDefaultValue = false)]
    public string? CancelReason { get; set; }
}
