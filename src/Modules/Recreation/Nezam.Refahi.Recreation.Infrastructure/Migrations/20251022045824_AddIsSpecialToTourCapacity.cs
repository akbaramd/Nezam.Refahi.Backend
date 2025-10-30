using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSpecialToTourCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSpecial",
                schema: "recreation",
                table: "TourCapacities",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if this capacity is only visible to special VIP members");

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacity_TenantTourSpecial",
                schema: "recreation",
                table: "TourCapacities",
                columns: new[] { "TenantId", "TourId", "IsSpecial" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TourCapacity_TenantTourSpecial",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropColumn(
                name: "IsSpecial",
                schema: "recreation",
                table: "TourCapacities");
        }
    }
}
