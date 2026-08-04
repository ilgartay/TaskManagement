using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using TaskManagement.API.Data;
using TaskManagement.API.Models;

namespace TaskManagement.API.Tests;

public sealed class DatabaseProviderTests
{
    [Fact]
    public void PostgreSql_Migrations_Produce_PostgreSql_Sql()
    {
        using var context = CreatePostgreSqlContext("Host=localhost;Database=script_test;Username=test;Password=test");

        var script = context.Database.GetService<IMigrator>().GenerateScript();

        Assert.Contains("CREATE TABLE", script);
        Assert.Contains("Users", script);
        Assert.Contains("uuid", script, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ADD \"Role\"", script);
    }

    [Fact]
    public void Oracle_Migrations_Produce_Oracle_Compatible_Sql()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseOracle("User Id=test;Password=test;Data Source=localhost:1521/XEPDB1")
            .Options;
        using var context = new ApplicationDbContext(options);

        var script = context.Database.GetService<IMigrator>().GenerateScript();

        Assert.Contains("CREATE TABLE", script);
        Assert.Contains("Users", script);
        Assert.Contains("RAW(16)", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(" uuid", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(" boolean", script, StringComparison.OrdinalIgnoreCase);
    }

    [PostgreSqlFact]
    public async Task PostgreSql_Migrations_And_Basic_Persistence_Work_On_A_Real_Database()
    {
        var connectionString = Environment.GetEnvironmentVariable("TEST_POSTGRES_CONNECTION")!;
        var connectionBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        Assert.EndsWith("_test", connectionBuilder.Database, StringComparison.OrdinalIgnoreCase);

        var schema = $"integration_{Guid.NewGuid():N}";
        connectionBuilder.SearchPath = schema;
        await using var connection = new NpgsqlConnection(connectionBuilder.ConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        await using (var schemaCommand = new NpgsqlCommand(
            $"CREATE SCHEMA \"{schema}\"; SET LOCAL search_path TO \"{schema}\";",
            connection,
            transaction))
        {
            await schemaCommand.ExecuteNonQueryAsync();
        }

        await using var context = CreatePostgreSqlContext(connection);
        await context.Database.UseTransactionAsync(transaction);
        var migrationScript = context.Database.GetService<IMigrator>().GenerateScript(
            options: MigrationsSqlGenerationOptions.NoTransactions);
        await context.Database.ExecuteSqlRawAsync(migrationScript);

        var user = new User
        {
            Username = $"postgres_{Guid.NewGuid():N}",
            Email = $"postgres_{Guid.NewGuid():N}@example.com",
            PasswordHash = "not-used-in-this-test",
            FirstName = "PostgreSQL",
            LastName = "Test"
        };
        var task = new TaskItem
        {
            Title = "PostgreSQL persistence test",
            User = user,
            Priority = Priority.High
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var savedTask = await context.Tasks.Include(item => item.User).SingleAsync(item => item.Id == task.Id);
        Assert.Equal(user.Username, savedTask.User.Username);

        await transaction.RollbackAsync();
    }

    private static ApplicationDbContext CreatePostgreSqlContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ApplicationDbContext(options);
    }

    private static ApplicationDbContext CreatePostgreSqlContext(NpgsqlConnection connection)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connection)
            .Options;

        return new ApplicationDbContext(options);
    }
}
