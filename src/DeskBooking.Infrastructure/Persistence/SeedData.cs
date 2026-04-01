
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DeskBooking.Contracts.Enums;
using DeskBooking.Domain.Entities;
using DeskBooking.Domain.Security;

namespace DeskBooking.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext dbContext, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Users.AnyAsync(cancellationToken))
        {
            dbContext.Users.AddRange(
                new User
                {
                    DisplayName = "System Administrator",
                    Email = "admin@room-booking.local",
                    PasswordHash = PasswordHasher.HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new User
                {
                    DisplayName = "Regular Employee",
                    Email = "employee@room-booking.local",
                    PasswordHash = PasswordHasher.HashPassword("Employee123!"),
                    Role = UserRole.Employee,
                    CreatedAtUtc = DateTime.UtcNow
                });

            await dbContext.SaveChangesAsync(cancellationToken);
            logger?.LogInformation("Тестовые пользователи добавлены.");
        }

        if (!await dbContext.Rooms.AnyAsync(cancellationToken))
        {            dbContext.Rooms.AddRange(
                new Room
                {
                    Name = "Desk-21",
                    Location = "Опенспейс, ряд C",
                    Capacity = 1,
                    HasProjector = true,
                    HasWhiteboard = true,
                    Description = "Место с монитором и док-станцией.",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Room
                {
                    Name = "Desk-08",
                    Location = "Тихая зона, ряд A",
                    Capacity = 1,
                    HasProjector = true,
                    HasWhiteboard = false,
                    Description = "Для фокуса, рядом с окном.",
                    CreatedAtUtc = DateTime.UtcNow
                });
await dbContext.SaveChangesAsync(cancellationToken);
            logger?.LogInformation("Тестовые комнаты добавлены.");
        }
    }
}
