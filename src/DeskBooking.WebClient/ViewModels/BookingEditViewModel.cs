
using System.ComponentModel.DataAnnotations;
using DeskBooking.Contracts.DataContracts;

namespace DeskBooking.WebClient.ViewModels;

public class BookingEditViewModel
{
    public int BookingId { get; set; }

    [Required(ErrorMessage = "Комната обязательна.")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "Тема обязательна.")]
    public string Title { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;

    [Required(ErrorMessage = "Дата начала обязательна.")]
    public DateTime StartLocal { get; set; }

    [Required(ErrorMessage = "Дата окончания обязательна.")]
    public DateTime EndLocal { get; set; }

    [Range(1, 1000, ErrorMessage = "Количество участников должно быть больше нуля.")]
    public int ParticipantCount { get; set; } = 1;

    public List<RoomDto> Rooms { get; set; } = new();
}
