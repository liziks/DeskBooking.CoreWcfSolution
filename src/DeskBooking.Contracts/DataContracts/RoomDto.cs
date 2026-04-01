
using System.Runtime.Serialization;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class RoomDto
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string Location { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public int Capacity { get; set; }

    [DataMember(Order = 5)]
    public bool HasProjector { get; set; }

    [DataMember(Order = 6)]
    public bool HasWhiteboard { get; set; }

    [DataMember(Order = 7)]
    public string Description { get; set; } = string.Empty;
}
