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
                columnTypes: new[]
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
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyColumnType: "uuid",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
