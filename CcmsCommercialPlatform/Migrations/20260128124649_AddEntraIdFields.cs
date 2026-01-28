using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CcmsCommercialPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddEntraIdFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntraClientId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EntraClientSecret",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EntraTenantId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntraClientId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");

            migrationBuilder.DropColumn(
                name: "EntraClientSecret",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");

            migrationBuilder.DropColumn(
                name: "EntraTenantId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");
        }
    }
}
