namespace TaskManagement.API.Tests;

public sealed class PostgreSqlFactAttribute : FactAttribute
{
    public PostgreSqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEST_POSTGRES_CONNECTION")))
        {
            Skip = "TEST_POSTGRES_CONNECTION tanımlanmadığı için canlı PostgreSQL testi atlandı.";
        }
    }
}
