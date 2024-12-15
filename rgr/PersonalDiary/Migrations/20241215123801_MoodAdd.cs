using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalDiary.Migrations
{
    /// <inheritdoc />
    public partial class MoodAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mood",
                table: "DiaryPosts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mood",
                table: "DiaryPosts");
        }
    }
}
