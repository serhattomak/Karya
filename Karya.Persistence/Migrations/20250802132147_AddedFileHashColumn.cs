using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class AddedFileHashColumn : Migration
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

			migrationBuilder.AddColumn<string>(
				name: "Hash",
				table: "Files",
				type: "nvarchar(64)",
				maxLength: 64,
				nullable: true);

			migrationBuilder.Sql(@"
                WITH NumberedFiles AS (
                    SELECT Id, 
                           CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 
                               CONCAT(ISNULL(Name, ''), '|', ISNULL(Path, ''), '|', 
                                     ISNULL(CONVERT(VARCHAR, Size), ''), '|', ISNULL(ContentType, ''), '|',
                                     CONVERT(VARCHAR, ROW_NUMBER() OVER (ORDER BY Id)))), 2) as NewHash
                    FROM Files
                    WHERE Hash IS NULL
                )
                UPDATE f 
                SET Hash = nf.NewHash
                FROM Files f
                INNER JOIN NumberedFiles nf ON f.Id = nf.Id");

			migrationBuilder.AlterColumn<string>(
				name: "Hash",
				table: "Files",
				type: "nvarchar(64)",
				maxLength: 64,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(64)",
				oldMaxLength: 64,
				oldNullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_Files_Hash",
				table: "Files",
				column: "Hash",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_Files_Hash",
				table: "Files");

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

			migrationBuilder.DropColumn(
				name: "Hash",
				table: "Files");
		}
	}
}
