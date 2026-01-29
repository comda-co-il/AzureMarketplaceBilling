using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CcmsCommercialPlatform.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCountryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only add CountryCode and CountryOther columns - skip column type changes that were SQLite-specific
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryOther",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");

            migrationBuilder.DropColumn(
                name: "CountryOther",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");
        }
    }
}
