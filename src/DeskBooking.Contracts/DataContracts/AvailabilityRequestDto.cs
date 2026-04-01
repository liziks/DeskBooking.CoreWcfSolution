
using System.Runtime.Serialization;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class AvailabilityRequestDto
{
    [DataMember(Order = 1)]
    public DateTime StartUtc { get; set; }

    [DataMember(Order = 2)]
    public DateTime EndUtc { get; set; }

    [DataMember(Order = 3, EmitDefaultValue = false)]
    public int? MinCapacity { get; set; }

    [DataMember(Order = 4)]
    public bool RequiresProjector { get; set; }

    [DataMember(Order = 5)]
    public bool RequiresWhiteboard { get; set; }
}
