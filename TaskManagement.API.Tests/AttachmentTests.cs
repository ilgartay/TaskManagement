using System.Net.Http.Json;
using System.Text;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;

namespace TaskManagement.API.Tests;

public sealed class AttachmentTests : IntegrationTestBase
{
    public AttachmentTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task File_Can_Be_Uploaded_Listed_And_Downloaded()
    {
        await RegisterAndAuthorizeAsync("file");
        var taskResponse = await Client.PostAsJsonAsync("/api/tasks", new CreateTaskDto
        {
            Title = "Dosya testi",
            Priority = Priority.Normal
        });
        taskResponse.EnsureSuccessStatusCode();
        var task = (await taskResponse.Content.ReadFromJsonAsync<TaskItemDto>())!;

        var expected = Encoding.UTF8.GetBytes("test attachment content");
        using var form = new MultipartFormDataContent();
        using var file = new ByteArrayContent(expected);
        file.Headers.ContentType = new("text/plain");
        form.Add(file, "file", "notes.txt");

        var uploadResponse = await Client.PostAsync($"/api/tasks/{task.Id}/attachments", form);
        uploadResponse.EnsureSuccessStatusCode();
        var attachment = (await uploadResponse.Content.ReadFromJsonAsync<TaskAttachmentDto>())!;

        var list = await Client.GetFromJsonAsync<List<TaskAttachmentDto>>(
            $"/api/tasks/{task.Id}/attachments");
        Assert.NotNull(list);
        Assert.Single(list);
        Assert.Equal("notes.txt", list[0].FileName);

        var downloaded = await Client.GetByteArrayAsync(
            $"/api/tasks/attachments/{attachment.Id}/download");
        Assert.Equal(expected, downloaded);
    }

    [Fact]
    public async Task File_Larger_Than_Ten_Megabytes_Is_Rejected()
    {
        await RegisterAndAuthorizeAsync("large-file");
        var taskResponse = await Client.PostAsJsonAsync("/api/tasks", new CreateTaskDto
        {
            Title = "Büyük dosya testi",
            Priority = Priority.Normal
        });
        taskResponse.EnsureSuccessStatusCode();
        var task = (await taskResponse.Content.ReadFromJsonAsync<TaskItemDto>())!;

        using var form = new MultipartFormDataContent();
        using var file = new ByteArrayContent(new byte[10 * 1024 * 1024 + 1]);
        file.Headers.ContentType = new("application/octet-stream");
        form.Add(file, "file", "large.bin");

        var response = await Client.PostAsync($"/api/tasks/{task.Id}/attachments", form);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }
}
