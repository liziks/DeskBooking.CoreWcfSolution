
using DeskBooking.Application.Services;
using DeskBooking.Contracts.Enums;
using DeskBooking.Domain.Entities;
using DeskBooking.Domain.Interfaces;
using DeskBooking.Domain.Security;
using Xunit;

namespace DeskBooking.Tests.Application;

public class AuthAppServiceTests
{
    [Fact]
    public async Task AuthenticateAsync_ReturnsSuccess_ForValidCredentials()
    {
        var user = new User
        {
            Id = 1,
            DisplayName = "Admin",
            Email = "admin@test.local",
            PasswordHash = PasswordHasher.HashPassword("Password123!"),
            Role = UserRole.Admin
        };

        var repository = new FakeUserRepository(user);
        var service = new AuthAppService(repository);

        var result = await service.AuthenticateAsync("admin@test.local", "Password123!");

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsFailure_ForWrongPassword()
    {
        var user = new User
        {
            Id = 1,
            DisplayName = "Admin",
            Email = "admin@test.local",
            PasswordHash = PasswordHasher.HashPassword("Password123!"),
            Role = UserRole.Admin
        };

        var repository = new FakeUserRepository(user);
        var service = new AuthAppService(repository);

        var result = await service.AuthenticateAsync("admin@test.local", "WrongPassword");

        Assert.False(result.Success);
        Assert.Equal("Неверный пароль.", result.Message);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly User? _user;

        public FakeUserRepository(User? user)
        {
            _user = user;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_user?.Email == email ? _user : null);
        }

        public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_user?.Id == userId ? _user : null);
        }
    }
}
