
using System.Runtime.Serialization;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class LoginRequestDto
{
    [DataMember(Order = 1)]
    public string Email { get; set; } = string.Empty;

    [DataMember(Order = 2)]
    public string Password { get; set; } = string.Empty;
}
