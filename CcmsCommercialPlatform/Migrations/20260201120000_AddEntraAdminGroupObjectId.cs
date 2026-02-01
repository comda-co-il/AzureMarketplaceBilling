using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CcmsCommercialPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddEntraAdminGroupObjectId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntraAdminGroupObjectId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntraAdminGroupObjectId",
                schema: "CcmsCommercialPlatform",
                table: "MarketplaceSubscriptions");
        }
    }
}
