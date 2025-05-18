using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Karya.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAboutHomeContactPageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutPages");

            migrationBuilder.DropTable(
                name: "ContactPages");

            migrationBuilder.DropTable(
                name: "HomePages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descriptions = table.Column<string>(type: "TEXT", nullable: false),
                    FileIds = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Titles = table.Column<string>(type: "TEXT", nullable: false),
                    Urls = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Addresses = table.Column<string>(type: "TEXT", nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Emails = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhoneNumbers = table.Column<string>(type: "TEXT", nullable: false),
                    SocialMediaLinks = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomePages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileIds = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProductIds = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePages", x => x.Id);
                });
        }
    }
}
