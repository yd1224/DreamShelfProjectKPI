using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamShelf.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueChapterIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Chapter_ChapterNumber_BookId",
                table: "Chapter",
                columns: new[] { "ChapterNumber", "BookId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chapter_ChapterNumber_BookId",
                table: "Chapter");
        }
    }
}
