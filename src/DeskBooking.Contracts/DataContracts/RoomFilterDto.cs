
using System.Runtime.Serialization;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class RoomFilterDto
{
    [DataMember(Order = 1, EmitDefaultValue = false)]
    public string? SearchTerm { get; set; }

    [DataMember(Order = 2, EmitDefaultValue = false)]
    public int? MinCapacity { get; set; }

    [DataMember(Order = 3)]
    public bool RequiresProjector { get; set; }

    [DataMember(Order = 4)]
    public bool RequiresWhiteboard { get; set; }
}
