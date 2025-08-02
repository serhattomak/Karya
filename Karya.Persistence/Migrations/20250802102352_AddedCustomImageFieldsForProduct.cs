using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCustomImageFieldsForProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrl",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentImageIds",
                table: "Products",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductDetailImageIds",
                table: "Products",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductImageId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerImageUrl",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DocumentImageIds",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductDetailImageIds",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductImageId",
                table: "Products");
        }
    }
}
