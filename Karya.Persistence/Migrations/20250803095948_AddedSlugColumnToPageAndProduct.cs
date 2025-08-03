using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class AddedSlugColumnToPageAndProduct : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Slug",
				table: "Products",
				type: "nvarchar(250)",
				maxLength: 250,
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Slug",
				table: "Pages",
				type: "nvarchar(250)",
				maxLength: 250,
				nullable: true);

			migrationBuilder.Sql(@"
                WITH ProductSlugs AS (
                    SELECT 
                        Id,
                        LOWER(
                            REPLACE(
                                REPLACE(
                                    REPLACE(
                                        REPLACE(
                                            REPLACE(
                                                REPLACE(
                                                    REPLACE(
                                                        REPLACE(
                                                            REPLACE(Name, 'ç', 'c'), 
                                                            'ğ', 'g'), 
                                                        'ı', 'i'), 
                                                    'ö', 'o'), 
                                                'ş', 's'), 
                                            'ü', 'u'), 
                                        ' ', '-'), 
                                    '.', ''), 
                                ',', '')
                        ) + '-' + CAST(ROW_NUMBER() OVER (ORDER BY CreatedDate) AS VARCHAR(10)) as Slug
                    FROM Products
                    WHERE Slug IS NULL
                )
                UPDATE p 
                SET Slug = ps.Slug
                FROM Products p
                INNER JOIN ProductSlugs ps ON p.Id = ps.Id");

			migrationBuilder.Sql(@"
                WITH PageSlugs AS (
                    SELECT 
                        Id,
                        LOWER(
                            REPLACE(
                                REPLACE(
                                    REPLACE(
                                        REPLACE(
                                            REPLACE(
                                                REPLACE(
                                                    REPLACE(
                                                        REPLACE(
                                                            REPLACE(Name, 'ç', 'c'), 
                                                            'ğ', 'g'), 
                                                        'ı', 'i'), 
                                                    'ö', 'o'), 
                                                'ş', 's'), 
                                            'ü', 'u'), 
                                        ' ', '-'), 
                                    '.', ''), 
                                ',', '')
                        ) + '-' + CAST(ROW_NUMBER() OVER (ORDER BY CreatedDate) AS VARCHAR(10)) as Slug
                    FROM Pages
                    WHERE Slug IS NULL
                )
                UPDATE p 
                SET Slug = ps.Slug
                FROM Pages p
                INNER JOIN PageSlugs ps ON p.Id = ps.Id");

			migrationBuilder.AlterColumn<string>(
				name: "Slug",
				table: "Products",
				type: "nvarchar(250)",
				maxLength: 250,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(250)",
				oldMaxLength: 250,
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "Slug",
				table: "Pages",
				type: "nvarchar(250)",
				maxLength: 250,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(250)",
				oldMaxLength: 250,
				oldNullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_Products_Slug",
				table: "Products",
				column: "Slug",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Pages_Slug",
				table: "Pages",
				column: "Slug",
				unique: true);

			migrationBuilder.Sql(@"
				IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Name' AND object_id = OBJECT_ID('Products'))
				BEGIN
					CREATE UNIQUE INDEX IX_Products_Name ON Products (Name);
				END");

			migrationBuilder.Sql(@"
				IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pages_Name' AND object_id = OBJECT_ID('Pages'))
				BEGIN
					CREATE UNIQUE INDEX IX_Pages_Name ON Pages (Name);
				END");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_Products_Slug",
				table: "Products");

			migrationBuilder.DropIndex(
				name: "IX_Pages_Slug",
				table: "Pages");

			migrationBuilder.Sql(@"
				IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Name' AND object_id = OBJECT_ID('Products'))
				BEGIN
					DROP INDEX IX_Products_Name ON Products;
				END");

			migrationBuilder.Sql(@"
				IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pages_Name' AND object_id = OBJECT_ID('Pages'))
				BEGIN
					DROP INDEX IX_Pages_Name ON Pages;
				END");

			migrationBuilder.DropColumn(
				name: "Slug",
				table: "Products");

			migrationBuilder.DropColumn(
				name: "Slug",
				table: "Pages");
		}
	}
}