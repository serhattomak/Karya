using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedMainImageIdToPageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MainImageId",
                table: "Pages",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainImageId",
                table: "Pages");
        }
    }
}
