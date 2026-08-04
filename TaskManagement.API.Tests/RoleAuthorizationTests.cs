using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;

namespace TaskManagement.API.Tests;

public sealed class RoleAuthorizationTests : IntegrationTestBase
{
    private readonly CustomWebApplicationFactory _factory;

    public RoleAuthorizationTests(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Admin_Endpoint_Separates_User_And_Admin_Roles()
    {
        var auth = await RegisterAndAuthorizeAsync("role");

        var userResponse = await Client.GetAsync("/api/admin/summary");
        Assert.Equal(HttpStatusCode.Forbidden, userResponse.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await dbContext.Users.SingleAsync(item => item.Id == auth.User.Id);
            user.Role = UserRole.Admin;
            await dbContext.SaveChangesAsync();
        }

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            UsernameOrEmail = auth.User.Email,
            Password = "Test123!"
        });
        loginResponse.EnsureSuccessStatusCode();
        var adminAuth = (await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>())!;
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminAuth.Token);

        var adminResponse = await Client.GetAsync("/api/admin/summary");
        adminResponse.EnsureSuccessStatusCode();
    }
}
