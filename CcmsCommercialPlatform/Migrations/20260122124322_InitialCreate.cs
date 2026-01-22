using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CcmsCommercialPlatform.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CcmsCommercialPlatform");

            migrationBuilder.CreateTable(
                name: "AzureWebhookEvents",
                schema: "CcmsCommercialPlatform",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RawPayload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ActivityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PublisherId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfferId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AzureTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceIp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AzureWebhookEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                schema: "CcmsCommercialPlatform",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPopular = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsageEvents",
                schema: "CcmsCommercialPlatform",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlanId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dimension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EffectiveStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanQuotas",
                schema: "CcmsCommercialPlatform",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DimensionType = table.Column<int>(type: "int", nullable: false),
                    DimensionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncludedQuantity = table.Column<int>(type: "int", nullable: false),
                    OveragePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanQuotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanQuotas_Plans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "CcmsCommercialPlatform",
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "CcmsCommercialPlatform",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillingPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillingPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Plans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "CcmsCommercialPlatform",
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsageRecords",
                schema: "CcmsCommercialPlatform",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DimensionType = table.Column<int>(type: "int", nullable: false),
                    DimensionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedQuantity = table.Column<int>(type: "int", nullable: false),
                    ReportedOverage = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsageRecords_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "CcmsCommercialPlatform",
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AzureWebhookEvents_Action",
                schema: "CcmsCommercialPlatform",
                table: "AzureWebhookEvents",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AzureWebhookEvents_ReceivedAt",
                schema: "CcmsCommercialPlatform",
                table: "AzureWebhookEvents",
                column: "ReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AzureWebhookEvents_SubscriptionId",
                schema: "CcmsCommercialPlatform",
                table: "AzureWebhookEvents",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanQuotas_PlanId",
                schema: "CcmsCommercialPlatform",
                table: "PlanQuotas",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanId",
                schema: "CcmsCommercialPlatform",
                table: "Subscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UsageEvents_CreatedAt",
                schema: "CcmsCommercialPlatform",
                table: "UsageEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UsageEvents_ResourceId",
                schema: "CcmsCommercialPlatform",
                table: "UsageEvents",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UsageRecords_SubscriptionId_DimensionType_BillingPeriodStart",
                schema: "CcmsCommercialPlatform",
                table: "UsageRecords",
                columns: new[] { "SubscriptionId", "DimensionType", "BillingPeriodStart" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AzureWebhookEvents",
                schema: "CcmsCommercialPlatform");

            migrationBuilder.DropTable(
                name: "PlanQuotas",
                schema: "CcmsCommercialPlatform");

            migrationBuilder.DropTable(
                name: "UsageEvents",
                schema: "CcmsCommercialPlatform");

            migrationBuilder.DropTable(
                name: "UsageRecords",
                schema: "CcmsCommercialPlatform");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "CcmsCommercialPlatform");

            migrationBuilder.DropTable(
                name: "Plans",
                schema: "CcmsCommercialPlatform");
        }
    }
}
