
using Microsoft.AspNetCore.Mvc;
using DeskBooking.Contracts.DataContracts;
using DeskBooking.WebClient.Services;
using DeskBooking.WebClient.ViewModels;

namespace DeskBooking.WebClient.Controllers;

public class RoomsController : AppControllerBase
{
    private readonly RoomApiClient _roomApiClient;

    public RoomsController(RoomApiClient roomApiClient)
    {
        _roomApiClient = roomApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, int? minCapacity, bool requiresProjector, bool requiresWhiteboard)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        var response = await _roomApiClient.GetAllAsync(SessionToken!, new RoomFilterDto
        {
            SearchTerm = searchTerm,
            MinCapacity = minCapacity,
            RequiresProjector = requiresProjector,
            RequiresWhiteboard = requiresWhiteboard
        });

        if (!response.Success)
        {
            SetError(response.Message);
        }

        ViewBag.SearchTerm = searchTerm;
        ViewBag.MinCapacity = minCapacity;
        ViewBag.RequiresProjector = requiresProjector;
        ViewBag.RequiresWhiteboard = requiresWhiteboard;

        return View(response.Rooms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        if (!IsAdmin)
        {
            SetError("Только администратор может создавать комнаты.");
            return RedirectToAction(nameof(Index));
        }

        return View(new RoomEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoomEditViewModel model)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        if (!IsAdmin)
        {
            SetError("Только администратор может создавать комнаты.");
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _roomApiClient.CreateAsync(SessionToken!, new RoomDto
        {
            Name = model.Name,
            Location = model.Location,
            Capacity = model.Capacity,
            HasProjector = model.HasProjector,
            HasWhiteboard = model.HasWhiteboard,
            Description = model.Description
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

        if (!IsAdmin)
        {
            SetError("Только администратор может редактировать комнаты.");
            return RedirectToAction(nameof(Index));
        }

        var response = await _roomApiClient.GetByIdAsync(SessionToken!, id);
        if (!response.Success || response.Room is null)
        {
            SetError(response.Message);
            return RedirectToAction(nameof(Index));
        }

        return View(new RoomEditViewModel
        {
            Id = response.Room.Id,
            Name = response.Room.Name,
            Location = response.Room.Location,
            Capacity = response.Room.Capacity,
            HasProjector = response.Room.HasProjector,
            HasWhiteboard = response.Room.HasWhiteboard,
            Description = response.Room.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoomEditViewModel model)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        if (!IsAdmin)
        {
            SetError("Только администратор может редактировать комнаты.");
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _roomApiClient.UpdateAsync(SessionToken!, new RoomDto
        {
            Id = model.Id,
            Name = model.Name,
            Location = model.Location,
            Capacity = model.Capacity,
            HasProjector = model.HasProjector,
            HasWhiteboard = model.HasWhiteboard,
            Description = model.Description
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
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAuthenticated)
        {
            return RedirectToLogin();
        }

        if (!IsAdmin)
        {
            SetError("Только администратор может удалять комнаты.");
            return RedirectToAction(nameof(Index));
        }

        var result = await _roomApiClient.DeleteAsync(SessionToken!, id);
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
}
