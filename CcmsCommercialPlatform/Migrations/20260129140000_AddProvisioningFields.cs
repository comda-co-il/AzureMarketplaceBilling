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
                name: "WhitelistIps",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IaCDeploymentId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CcmsUrl",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "nvarchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvisioningMetadata",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvisioningErrorMessage",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProvisioningRequestedAt",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProvisioningCompletedAt",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AzureActivatedAt",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhitelistIps",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");

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

            migrationBuilder.DropColumn(
                name: "AzureActivatedAt",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");
        }
    }
}
