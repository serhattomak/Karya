using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubtitleAndListItemsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                table: "Products",
                type: "NVARCHAR(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.AddColumn<string>(
                name: "ListItems",
                table: "Products",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subtitles",
                table: "Products",
                type: "NVARCHAR(MAX)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListItems",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Subtitles",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Descriptions",
                table: "Products",
                type: "NVARCHAR(MAX)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)",
                oldNullable: true);
        }
    }
}
