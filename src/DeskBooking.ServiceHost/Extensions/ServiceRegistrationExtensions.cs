using CoreWCF.Configuration;
using Microsoft.EntityFrameworkCore;
using DeskBooking.Application.Services;
using DeskBooking.Domain.Interfaces;
using DeskBooking.Infrastructure.Persistence;
using DeskBooking.Infrastructure.Repositories;
using DeskBooking.ServiceHost.Security;
using DeskBooking.ServiceHost.Services;

namespace DeskBooking.ServiceHost.Extensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddDeskBookingServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Data Source=room_booking.db";

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddScoped<AuthAppService>();
        services.AddScoped<RoomAppService>();
        services.AddScoped<BookingAppService>();

        services.AddSingleton<ISessionManager, InMemorySessionManager>();

        services.AddScoped<AuthService>();
        services.AddScoped<RoomService>();
        services.AddScoped<BookingService>();

        services.AddServiceModelServices();
        services.AddServiceModelMetadata();

        return services;
    }
}
