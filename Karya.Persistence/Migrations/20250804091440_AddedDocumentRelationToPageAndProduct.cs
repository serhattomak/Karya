using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedDocumentRelationToPageAndProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentIds",
                table: "Products",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentIds",
                table: "Pages",
                type: "NVARCHAR(MAX)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentIds",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DocumentIds",
                table: "Pages");
        }
    }
}
