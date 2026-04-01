using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.EntityFrameworkCore;
using DeskBooking.Infrastructure.Persistence;
using DeskBooking.ServiceHost.Extensions;
using DeskBooking.ServiceHost.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5050");
builder.Services.AddDeskBookingServices(builder.Configuration);

var app = builder.Build();

var metadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
metadataBehavior.HttpGetEnabled = true;

app.UseServiceModel(serviceBuilder =>
{
    var binding = new BasicHttpBinding
    {
        MaxReceivedMessageSize = int.MaxValue
    };

    serviceBuilder.AddService<AuthService>();
    serviceBuilder.AddServiceEndpoint<AuthService, DeskBooking.Contracts.ServiceContracts.IAuthService>(
        binding,
        "/Services/AuthService.svc");

    serviceBuilder.AddService<RoomService>();
    serviceBuilder.AddServiceEndpoint<RoomService, DeskBooking.Contracts.ServiceContracts.IRoomService>(
        binding,
        "/Services/RoomService.svc");

    serviceBuilder.AddService<BookingService>();
    serviceBuilder.AddServiceEndpoint<BookingService, DeskBooking.Contracts.ServiceContracts.IBookingService>(
        binding,
        "/Services/BookingService.svc");
});

app.MapGet("/", () => Results.Text(
    "DeskBooking.ServiceHost запущен. Откройте /Services/AuthService.svc?wsdl, /Services/RoomService.svc?wsdl или /Services/BookingService.svc?wsdl"));

await TryInitializeDatabaseAsync(app.Services, app.Logger);

app.Run();

static async Task TryInitializeDatabaseAsync(IServiceProvider services, ILogger logger)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();
        if (!appliedMigrations.Any())
        {
            logger.LogWarning("Не найдено применённых миграций. Сначала выполни Add-Migration и Update-Database.");
            return;
        }

        await SeedData.InitializeAsync(dbContext, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ошибка инициализации БД.");
    }
}
