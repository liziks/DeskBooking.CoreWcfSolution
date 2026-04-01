
using System.Runtime.Serialization;
using DeskBooking.Contracts.Enums;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class LoginResponseDto
{
    [DataMember(Order = 1)]
    public bool Success { get; set; }

    [DataMember(Order = 2)]
    public string Message { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public string SessionToken { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public int UserId { get; set; }

    [DataMember(Order = 5)]
    public string DisplayName { get; set; } = string.Empty;

    [DataMember(Order = 6)]
    public UserRole Role { get; set; }
}
