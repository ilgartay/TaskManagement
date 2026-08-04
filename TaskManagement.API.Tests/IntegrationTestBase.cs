using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Tests;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    protected HttpClient Client { get; }

    protected async Task<AuthResponseDto> RegisterAndAuthorizeAsync(string prefix)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var response = await Client.PostAsJsonAsync("/api/auth/register", new CreateUserDto
        {
            Username = $"{prefix}_{suffix}",
            Email = $"{prefix}_{suffix}@example.com",
            FirstName = "Test",
            LastName = "User",
            Password = "Test123!"
        });

        response.EnsureSuccessStatusCode();
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(auth);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        return auth;
    }
}
