using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;
using TaskStatus = TaskManagement.API.Models.TaskStatus;

namespace TaskManagement.API.Tests;

public sealed class TaskCrudTests : IntegrationTestBase
{
    public TaskCrudTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Task_Crud_Filter_Search_And_Pagination_Work()
    {
        await RegisterAndAuthorizeAsync("crud");
        var created = await CreateTaskAsync("Rapor hazırla", Priority.High);

        var getResponse = await Client.GetAsync($"/api/tasks/{created.Id}");
        getResponse.EnsureSuccessStatusCode();

        var updateResponse = await Client.PutAsJsonAsync($"/api/tasks/{created.Id}", new UpdateTaskDto
        {
            Title = "Haftalık raporu hazırla",
            Description = "Test ile güncellendi",
            Priority = Priority.Critical,
            Status = TaskStatus.Completed
        });
        updateResponse.EnsureSuccessStatusCode();

        var filterResponse = await Client.GetAsync(
            "/api/tasks/filter?searchTerm=Haftalık&page=1&pageSize=1&status=2");
        filterResponse.EnsureSuccessStatusCode();
        var filtered = await filterResponse.Content.ReadFromJsonAsync<List<TaskItemDto>>();

        Assert.NotNull(filtered);
        Assert.Single(filtered);
        Assert.Equal(TaskStatus.Completed, filtered[0].Status);

        var invalidPageSize = await Client.GetAsync("/api/tasks/filter?pageSize=101");
        Assert.Equal(HttpStatusCode.BadRequest, invalidPageSize.StatusCode);

        var deleteResponse = await Client.DeleteAsync($"/api/tasks/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync($"/api/tasks/{created.Id}")).StatusCode);
    }

    [Fact]
    public async Task Users_Cannot_Read_Each_Others_Tasks()
    {
        var firstUser = await RegisterAndAuthorizeAsync("owner");
        var task = await CreateTaskAsync("Sadece sahibine ait", Priority.Normal);

        var secondUser = await RegisterAndAuthorizeAsync("other");
        Assert.NotEqual(firstUser.User.Id, secondUser.User.Id);

        var response = await Client.GetAsync($"/api/tasks/{task.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Duplicate_Registration_Returns_Conflict()
    {
        var auth = await RegisterAndAuthorizeAsync("duplicate");
        Client.DefaultRequestHeaders.Authorization = null;

        var response = await Client.PostAsJsonAsync("/api/auth/register", new CreateUserDto
        {
            Username = auth.User.Username,
            Email = "another@example.com",
            FirstName = "Duplicate",
            LastName = "User",
            Password = "Test123!"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private async Task<TaskItemDto> CreateTaskAsync(string title, Priority priority)
    {
        var response = await Client.PostAsJsonAsync("/api/tasks", new CreateTaskDto
        {
            Title = title,
            Description = "Integration test task",
            Priority = priority,
            DueDate = DateTime.UtcNow.AddDays(2)
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TaskItemDto>())!;
    }
}
