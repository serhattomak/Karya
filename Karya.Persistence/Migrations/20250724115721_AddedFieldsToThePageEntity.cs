using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedFieldsToThePageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                table: "Pages",
                type: "NVARCHAR(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImageUrl",
                table: "Pages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrl",
                table: "Pages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ListItems",
                table: "Pages",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subtitles",
                table: "Pages",
                type: "NVARCHAR(MAX)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImageUrl",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "BannerImageUrl",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ListItems",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Subtitles",
                table: "Pages");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                table: "Pages",
                type: "NVARCHAR(MAX)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)",
                oldNullable: true);
        }
    }
}
