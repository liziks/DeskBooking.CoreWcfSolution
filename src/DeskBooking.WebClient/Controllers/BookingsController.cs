
using Microsoft.AspNetCore.Mvc;
using DeskBooking.Contracts.DataContracts;
using DeskBooking.WebClient.Infrastructure;
using DeskBooking.WebClient.Services;
using DeskBooking.WebClient.ViewModels;

namespace DeskBooking.WebClient.Controllers;

public class BookingsController : AppControllerBase
{
    private readonly BookingApiClient _bookingApiClient;
    private readonly RoomApiClient _roomApiClient;

    public BookingsController(BookingApiClient bookingApiClient, RoomApiClient roomApiClient)
    {
        _bookingApiClient = bookingApiClient;
        _roomApiClient = roomApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? fromLocal, DateTime? toLocal, int? roomId, bool onlyMyBookings = false)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        var roomsResponse = await _roomApiClient.GetAllAsync(SessionToken!, new RoomFilterDto());
        var bookingsResponse = await _bookingApiClient.GetAllAsync(SessionToken!, new BookingFilterDto
        {
            FromUtc = fromLocal.HasValue ? DateTimeLocalHelper.ToUtc(fromLocal.Value) : null,
            ToUtc = toLocal.HasValue ? DateTimeLocalHelper.ToUtc(toLocal.Value) : null,
            RoomId = roomId,
            OnlyMyBookings = onlyMyBookings
        });

        if (!bookingsResponse.Success)
        {
            SetError(bookingsResponse.Message);
        }

        var model = new BookingIndexViewModel
        {
            FromLocal = fromLocal,
            ToLocal = toLocal,
            RoomId = roomId,
            OnlyMyBookings = onlyMyBookings,
            Rooms = roomsResponse.Rooms,
            Bookings = bookingsResponse.Bookings
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        var roomsResponse = await _roomApiClient.GetAllAsync(SessionToken!, new RoomFilterDto());

        return View(new BookingEditViewModel
        {
            StartLocal = DateTime.Now.AddHours(1),
            EndLocal = DateTime.Now.AddHours(2),
            Rooms = roomsResponse.Rooms
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingEditViewModel model)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        var roomsResponse = await _roomApiClient.GetAllAsync(SessionToken!, new RoomFilterDto());
        model.Rooms = roomsResponse.Rooms;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _bookingApiClient.CreateAsync(SessionToken!, new CreateBookingRequestDto
        {
            RoomId = model.RoomId,
            Title = model.Title,
            Purpose = model.Purpose,
            StartUtc = DateTimeLocalHelper.ToUtc(model.StartLocal),
            EndUtc = DateTimeLocalHelper.ToUtc(model.EndLocal),
            ParticipantCount = model.ParticipantCount
        });

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        SetSuccess(result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        var bookingResponse = await _bookingApiClient.GetByIdAsync(SessionToken!, id);
        if (!bookingResponse.Success || bookingResponse.Booking is null)
        {
            SetError(bookingResponse.Message);
            return RedirectToAction(nameof(Index));
        }

        var roomsResponse = await _roomApiClient.GetAllAsync(SessionToken!, new RoomFilterDto());

        return View(new BookingEditViewModel
        {
            BookingId = bookingResponse.Booking.Id,
            RoomId = bookingResponse.Booking.RoomId,
            Title = bookingResponse.Booking.Title,
            Purpose = bookingResponse.Booking.Purpose,
            StartLocal = DateTimeLocalHelper.ToLocal(bookingResponse.Booking.StartUtc),
            EndLocal = DateTimeLocalHelper.ToLocal(bookingResponse.Booking.EndUtc),
            ParticipantCount = bookingResponse.Booking.ParticipantCount,
            Rooms = roomsResponse.Rooms
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookingEditViewModel model)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        var roomsResponse = await _roomApiClient.GetAllAsync(SessionToken!, new RoomFilterDto());
        model.Rooms = roomsResponse.Rooms;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _bookingApiClient.UpdateAsync(SessionToken!, new UpdateBookingRequestDto
        {
            BookingId = model.BookingId,
            RoomId = model.RoomId,
            Title = model.Title,
            Purpose = model.Purpose,
            StartUtc = DateTimeLocalHelper.ToUtc(model.StartLocal),
            EndUtc = DateTimeLocalHelper.ToUtc(model.EndLocal),
            ParticipantCount = model.ParticipantCount
        });

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        SetSuccess(result.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string? reason)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        var result = await _bookingApiClient.CancelAsync(SessionToken!, id, reason);

        if (result.Success)
        {
            SetSuccess(result.Message);
        }
        else
        {
            SetError(result.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Availability(DateTime? startLocal, DateTime? endLocal, int? minCapacity, bool requiresProjector, bool requiresWhiteboard)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        var model = new AvailabilitySearchViewModel
        {
            StartLocal = startLocal,
            EndLocal = endLocal,
            MinCapacity = minCapacity,
            RequiresProjector = requiresProjector,
            RequiresWhiteboard = requiresWhiteboard
        };

        if (startLocal.HasValue && endLocal.HasValue)
        {
            var response = await _bookingApiClient.GetAvailableRoomsAsync(SessionToken!, new AvailabilityRequestDto
            {
                StartUtc = DateTimeLocalHelper.ToUtc(startLocal.Value),
                EndUtc = DateTimeLocalHelper.ToUtc(endLocal.Value),
                MinCapacity = minCapacity,
                RequiresProjector = requiresProjector,
                RequiresWhiteboard = requiresWhiteboard
            });

            if (!response.Success)
            {
                SetError(response.Message);
            }
            else
            {
                model.AvailableRooms = response.Rooms;
            }
        }

        return View(model);
    }
}
