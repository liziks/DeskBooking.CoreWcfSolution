
using DeskBooking.WebClient.Infrastructure;
using DeskBooking.WebClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5051");

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
});

builder.Services.Configure<SoapEndpointOptions>(builder.Configuration.GetSection("SoapEndpoints"));
builder.Services.AddScoped<SoapClientExecutor>();
builder.Services.AddScoped<AuthApiClient>();
builder.Services.AddScoped<RoomApiClient>();
builder.Services.AddScoped<BookingApiClient>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
