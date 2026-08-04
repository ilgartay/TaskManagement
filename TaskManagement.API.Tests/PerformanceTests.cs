using System.Diagnostics;
using System.Net.Http.Json;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;

namespace TaskManagement.API.Tests;

public sealed class PerformanceTests : IntegrationTestBase
{
    public PerformanceTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Filtered_Task_Query_Completes_Within_Reasonable_Time()
    {
        await RegisterAndAuthorizeAsync("performance");

        for (var index = 0; index < 40; index++)
        {
            var response = await Client.PostAsJsonAsync("/api/tasks", new CreateTaskDto
            {
                Title = $"Performance task {index:00}",
                Priority = Priority.Normal
            });
            response.EnsureSuccessStatusCode();
        }

        var stopwatch = Stopwatch.StartNew();
        var result = await Client.GetFromJsonAsync<List<TaskItemDto>>(
            "/api/tasks/filter?searchTerm=Performance&page=1&pageSize=10");
        stopwatch.Stop();

        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(3),
            $"Filtre sorgusu {stopwatch.Elapsed.TotalMilliseconds:N0} ms sürdü.");
    }
}
