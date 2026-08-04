using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var oracle = ActiveProvider.Contains("Oracle", StringComparison.OrdinalIgnoreCase);
            var guidType = oracle ? "RAW(16)" : "uuid";
            var dateTimeType = oracle ? "TIMESTAMP(7)" : "timestamp with time zone";
            var boolType = oracle ? "NUMBER(1)" : "boolean";
            var intType = oracle ? "NUMBER(10)" : "integer";
            var longType = oracle ? "NUMBER(19)" : "bigint";
            string TextType(int length) => oracle
                ? $"NVARCHAR2({length})"
                : $"character varying({length})";

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: guidType, nullable: false),
                    Username = table.Column<string>(type: TextType(100), maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: TextType(200), maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: TextType(200), maxLength: 200, nullable: false),
                    FirstName = table.Column<string>(type: TextType(100), maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: TextType(100), maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: dateTimeType, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: dateTimeType, nullable: false),
                    IsActive = table.Column<bool>(type: boolType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: guidType, nullable: false),
                    Name = table.Column<string>(type: TextType(100), maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: TextType(500), maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: TextType(20), maxLength: 20, nullable: false),
                    UserId = table.Column<Guid>(type: guidType, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: dateTimeType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: guidType, nullable: false),
                    Title = table.Column<string>(type: TextType(200), maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: TextType(2000), maxLength: 2000, nullable: true),
                    Priority = table.Column<int>(type: intType, nullable: false),
                    Status = table.Column<int>(type: intType, nullable: false),
                    DueDate = table.Column<DateTime>(type: dateTimeType, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: dateTimeType, nullable: true),
                    UserId = table.Column<Guid>(type: guidType, nullable: false),
                    CategoryId = table.Column<Guid>(type: guidType, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: dateTimeType, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: dateTimeType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: guidType, nullable: false),
                    TaskId = table.Column<Guid>(type: guidType, nullable: false),
                    FileName = table.Column<string>(type: TextType(255), maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: TextType(1000), maxLength: 1000, nullable: false),
                    FileSize = table.Column<long>(type: longType, nullable: false),
                    ContentType = table.Column<string>(type: TextType(100), maxLength: 100, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: dateTimeType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAttachments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: guidType, nullable: false),
                    TaskId = table.Column<Guid>(type: guidType, nullable: false),
                    UserId = table.Column<Guid>(type: guidType, nullable: false),
                    Comment = table.Column<string>(type: TextType(2000), maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: dateTimeType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_TaskId",
                table: "TaskAttachments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_TaskId",
                table: "TaskComments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_UserId",
                table: "TaskComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CategoryId",
                table: "Tasks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskAttachments");

            migrationBuilder.DropTable(
                name: "TaskComments");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
