using System.Net;
using System.Net.Http.Json;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Tests;

public sealed class AuthFlowTests : IntegrationTestBase
{
    public AuthFlowTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_Login_And_Profile_Flow_Works()
    {
        var auth = await RegisterAndAuthorizeAsync("auth");

        var profileResponse = await Client.GetAsync("/api/auth/profile");
        profileResponse.EnsureSuccessStatusCode();
        var profile = await profileResponse.Content.ReadFromJsonAsync<UserDto>();

        Assert.NotNull(profile);
        Assert.Equal(auth.User.Id, profile.Id);

        Client.DefaultRequestHeaders.Authorization = null;
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            UsernameOrEmail = auth.User.Email,
            Password = "Test123!"
        });

        loginResponse.EnsureSuccessStatusCode();
        Assert.NotNull(await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>());
    }

    [Fact]
    public async Task Invalid_Login_Returns_Unauthorized()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            UsernameOrEmail = "missing-user",
            Password = "wrong-password"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Protected_Endpoint_Rejects_Anonymous_Request()
    {
        Client.DefaultRequestHeaders.Authorization = null;

        var response = await Client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_Registration_Model_Returns_Bad_Request()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/register", new CreateUserDto
        {
            Username = "x",
            Email = "invalid-email",
            FirstName = "",
            LastName = "",
            Password = "123"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
