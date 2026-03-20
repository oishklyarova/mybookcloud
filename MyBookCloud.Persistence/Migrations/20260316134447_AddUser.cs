using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBookCloud.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            // Seed admin user
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "CreatedAt" },
                values: new object[]
                {
                    new Guid("00000000-0000-0000-0000-000000000001"),
                    "admin@test.com",
                    // BCrypt hash for password: admin123
                    "$2a$11$3n3/Dt6ow5z9QQPV76m.OeEMSwD/Totw3qQndM6Yqk4ZsuuBm.7/S",
                    DateTime.UtcNow
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
