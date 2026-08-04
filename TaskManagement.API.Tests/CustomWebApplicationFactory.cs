using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.API.Data;

namespace TaskManagement.API.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly string _uploadPath = Path.Combine(
        Path.GetTempPath(),
        $"task-management-tests-{Guid.NewGuid():N}");

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _connection.DisposeAsync();

        if (Directory.Exists(_uploadPath))
        {
            Directory.Delete(_uploadPath, recursive: true);
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("DatabaseProvider", "PostgreSQL");
        builder.UseSetting("ConnectionStrings:PostgreSQLConnection", "Host=unused");
        builder.UseSetting("Jwt:Key", "integration-tests-only-secret-key-32-bytes-minimum");
        builder.UseSetting("Jwt:Issuer", "TaskManagementTests");
        builder.UseSetting("Jwt:Audience", "TaskManagementTests");
        builder.UseSetting("Jwt:ExpiresInMinutes", "15");
        builder.UseSetting("Storage:UploadPath", _uploadPath);
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DatabaseProvider"] = "PostgreSQL",
                ["ConnectionStrings:PostgreSQLConnection"] = "Host=unused",
                ["Jwt:Key"] = "integration-tests-only-secret-key-32-bytes-minimum",
                ["Jwt:Issuer"] = "TaskManagementTests",
                ["Jwt:Audience"] = "TaskManagementTests",
                ["Jwt:ExpiresInMinutes"] = "15",
                ["Storage:UploadPath"] = _uploadPath
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
        });
    }
}
