using System.Net;

namespace TaskManagement.API.Tests;

public sealed class CorsTests : IntegrationTestBase
{
    public CorsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Angular_Development_Origin_Is_Allowed()
    {
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        request.Headers.Add("Origin", "http://localhost:4200");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(
            "http://localhost:4200",
            response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }
}
