using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CcmsCommercialPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddProvisioningFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IaCDeploymentId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CcmsUrl",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvisioningMetadata",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvisioningErrorMessage",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProvisioningRequestedAt",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProvisioningCompletedAt",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IaCDeploymentId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");

            migrationBuilder.DropColumn(
                name: "CcmsUrl",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");

            migrationBuilder.DropColumn(
                name: "ProvisioningMetadata",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");

            migrationBuilder.DropColumn(
                name: "ProvisioningErrorMessage",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");

            migrationBuilder.DropColumn(
                name: "ProvisioningRequestedAt",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");

            migrationBuilder.DropColumn(
                name: "ProvisioningCompletedAt",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");
        }
    }
}
