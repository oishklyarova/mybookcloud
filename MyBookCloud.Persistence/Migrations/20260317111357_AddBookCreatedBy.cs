using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBookCloud.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Books",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.CreateIndex(
                name: "IX_Books_CreatedById",
                table: "Books",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_CreatedById",
                table: "Books",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] {
                    "Id", "Title", "Author", "Isbn", "AverageRating", "Note",
                    "CreatedAt", "PersonalRating", "Status", "CoverThumbnailUrl",
                    "PageCount", "CreatedById"
                            },
                            values: new object[,]
                            {
                    {
                        new Guid("3e3f0426-2397-4941-a97d-06bc688cb496"),
                        "The Seven Husbands of Evelyn Hugo",
                        "Taylor Jenkins Reid",
                        "9781501139246",
                        null, // AverageRating
                        null, // Note
                        DateTimeOffset.Parse("2026-03-15 19:27:24.400957+00"),
                        null, // PersonalRating
                        2,
                        "http://books.google.com/books/content?id=njVpDQAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api",
                        384,
                        new Guid("00000000-0000-0000-0000-000000000001")
                    },
                    {
                        new Guid("63a1fb14-9f3c-4926-b4bd-d735581b607e"),
                        "Alice in Wonderland",
                        "Lewis Carroll",
                        "9781840227802",
                        null, // AverageRating
                        null, // Note
                        DateTimeOffset.Parse("2026-03-14 16:07:21.614288+00"),
                        5,
                        2,
                        "http://books.google.com/books/content?id=-Xd3tgEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api",
                        160,
                        new Guid("00000000-0000-0000-0000-000000000001")
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_CreatedById",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_CreatedById",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Books");
        }
    }
}
