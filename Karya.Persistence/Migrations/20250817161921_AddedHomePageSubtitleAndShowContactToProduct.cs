using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedHomePageSubtitleAndShowContactToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HomePageSubtitle",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowContact",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomePageSubtitle",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShowContact",
                table: "Products");
        }
    }
}
