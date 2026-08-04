using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TaskManagement.API.Data;

#nullable disable

namespace TaskManagement.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260724115000_AddDemoUser")]
    public partial class AddDemoUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var oracle = ActiveProvider.Contains("Oracle", StringComparison.OrdinalIgnoreCase);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]
                {
                    "Id",
                    "CreatedAt",
                    "Email",
                    "FirstName",
                    "IsActive",
                    "LastName",
                    "PasswordHash",
                    "UpdatedAt",
                    "Username"
                },
                columnTypes: oracle
                    ? new[]
                    {
                        "RAW(16)",
                        "TIMESTAMP(7)",
                        "NVARCHAR2(2000)",
                        "NVARCHAR2(2000)",
                        "NUMBER(1)",
                        "NVARCHAR2(2000)",
                        "NVARCHAR2(2000)",
                        "TIMESTAMP(7)",
                        "NVARCHAR2(2000)"
                    }
                    : new[]
                    {
                        "uuid",
                        "timestamp with time zone",
                        "text",
                        "text",
                        "boolean",
                        "text",
                        "text",
                        "timestamp with time zone",
                        "text"
                    },
                values: new object[]
                {
                    new Guid("11111111-1111-1111-1111-111111111111"),
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    "demo@example.com",
                    "Demo",
                    true,
                    "User",
                    "$2a$12$cGT9TW5Yj0qase79ysDFju.BCTU3/xUjXlUSrPTyL0RHupdWLcqku",
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    "demo"
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var keyColumnType = ActiveProvider.Contains("Oracle", StringComparison.OrdinalIgnoreCase)
                ? "RAW(16)"
                : "uuid";

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyColumnType: keyColumnType,
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
