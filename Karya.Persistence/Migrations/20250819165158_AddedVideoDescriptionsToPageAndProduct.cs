using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class AddedVideoDescriptionsToPageAndProduct : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "VideoDescriptions",
				table: "Products",
				type: "NVARCHAR(MAX)",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "VideoDescriptions",
				table: "Pages",
				type: "NVARCHAR(MAX)",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "VideoDescriptions",
				table: "Products");

			migrationBuilder.DropColumn(
				name: "VideoDescriptions",
				table: "Pages");
		}
	}
}
