using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TaskManagement.API.Data;

#nullable disable

namespace TaskManagement.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260804171500_NormalizeStringLengths")]
    public partial class NormalizeStringLengths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var oracle = ActiveProvider.Contains("Oracle", StringComparison.OrdinalIgnoreCase);

            void Limit(string table, string column, int length, bool nullable = false)
            {
                migrationBuilder.AlterColumn<string>(
                    name: column,
                    table: table,
                    type: oracle ? $"NVARCHAR2({length})" : $"character varying({length})",
                    maxLength: length,
                    nullable: nullable,
                    oldClrType: typeof(string),
                    oldType: oracle ? "NVARCHAR2(2000)" : "text",
                    oldNullable: nullable);
            }

            Limit("Users", "Username", 100);
            Limit("Users", "Email", 200);
            Limit("Users", "PasswordHash", 200);
            Limit("Users", "FirstName", 100);
            Limit("Users", "LastName", 100);
            Limit("Categories", "Name", 100);
            Limit("Categories", "Description", 500, nullable: true);
            Limit("Categories", "Color", 20);
            Limit("Tasks", "Title", 200);
            Limit("Tasks", "Description", 2000, nullable: true);
            Limit("TaskAttachments", "FileName", 255);
            Limit("TaskAttachments", "FilePath", 1000);
            Limit("TaskAttachments", "ContentType", 100);
            Limit("TaskComments", "Comment", 2000);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var oracle = ActiveProvider.Contains("Oracle", StringComparison.OrdinalIgnoreCase);

            void RemoveLimit(string table, string column, int oldLength, bool nullable = false)
            {
                migrationBuilder.AlterColumn<string>(
                    name: column,
                    table: table,
                    type: oracle ? "NVARCHAR2(2000)" : "text",
                    nullable: nullable,
                    oldClrType: typeof(string),
                    oldType: oracle ? $"NVARCHAR2({oldLength})" : $"character varying({oldLength})",
                    oldMaxLength: oldLength,
                    oldNullable: nullable);
            }

            RemoveLimit("Users", "Username", 100);
            RemoveLimit("Users", "Email", 200);
            RemoveLimit("Users", "PasswordHash", 200);
            RemoveLimit("Users", "FirstName", 100);
            RemoveLimit("Users", "LastName", 100);
            RemoveLimit("Categories", "Name", 100);
            RemoveLimit("Categories", "Description", 500, nullable: true);
            RemoveLimit("Categories", "Color", 20);
            RemoveLimit("Tasks", "Title", 200);
            RemoveLimit("Tasks", "Description", 2000, nullable: true);
            RemoveLimit("TaskAttachments", "FileName", 255);
            RemoveLimit("TaskAttachments", "FilePath", 1000);
            RemoveLimit("TaskAttachments", "ContentType", 100);
            RemoveLimit("TaskComments", "Comment", 2000);
        }
    }
}
