
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeskBooking.WebClient.ViewModels;

public class RoomEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название обязательно.")]

    [DisplayName("Название")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Локация обязательна.")]

    [DisplayName("Локация")]
    public string Location { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "Вместимость должна быть больше нуля.")]

    [DisplayName("Кол-во мест")]
    public int Capacity { get; set; }

    [DisplayName("Монитор")]
    public bool HasProjector { get; set; }

    [DisplayName("Док-станция")]
    public bool HasWhiteboard { get; set; }
        [DisplayName("Описание")]
    public string Description { get; set; } = string.Empty;
}
