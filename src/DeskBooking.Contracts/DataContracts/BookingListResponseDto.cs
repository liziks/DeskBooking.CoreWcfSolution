
using System.Runtime.Serialization;

namespace DeskBooking.Contracts.DataContracts;

[DataContract(Namespace = ContractNamespaces.Data)]
public class BookingListResponseDto
{
    [DataMember(Order = 1)]
    public bool Success { get; set; }

    [DataMember(Order = 2)]
    public string Message { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    public List<BookingDto> Bookings { get; set; } = new();
}
