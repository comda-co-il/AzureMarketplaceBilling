using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CcmsCommercialPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketplaceSubscriptions",
                schema: "CcmsCommercialPlatform",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AzureSubscriptionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MarketplaceToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfferId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaserTenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaserObjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeneficiaryEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeneficiaryTenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeneficiaryObjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TokenResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerInfoSubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeatureSelectionCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedToExternalSystemAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureSelections",
                schema: "CcmsCommercialPlatform",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketplaceSubscriptionId = table.Column<int>(type: "int", nullable: false),
                    FeatureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FeatureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureSelections_MarketplaceSubscriptions_MarketplaceSubscriptionId",
                        column: x => x.MarketplaceSubscriptionId,
                        principalSchema: "CcmsCommercialPlatform",
                        principalTable: "MarketplaceSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureSelections_MarketplaceSubscriptionId_FeatureId",
                schema: "CcmsCommercialPlatform",
                table: "FeatureSelections",
                columns: new[] { "MarketplaceSubscriptionId", "FeatureId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceSubscriptions_AzureSubscriptionId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                column: "AzureSubscriptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceSubscriptions_CreatedAt",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceSubscriptions_Status",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeatureSelections",
                schema: "CcmsCommercialPlatform");

            migrationBuilder.DropTable(
                name: "MarketplaceSubscriptions",
                schema: "CcmsCommercialPlatform");
        }
    }
}
