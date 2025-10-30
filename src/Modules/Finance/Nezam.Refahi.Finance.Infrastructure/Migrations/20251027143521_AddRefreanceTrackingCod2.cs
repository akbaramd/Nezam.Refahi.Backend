using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreanceTrackingCod2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_BillNumber",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BillNumber",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CallbackUrl",
                schema: "finance",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "GatewayReference",
                schema: "finance",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_GatewayReference",
                schema: "finance",
                table: "Payments",
                column: "GatewayReference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_GatewayReference",
                schema: "finance",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "GatewayReference",
                schema: "finance",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BillNumber",
                schema: "finance",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CallbackUrl",
                schema: "finance",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillNumber",
                schema: "finance",
                table: "Payments",
                column: "BillNumber");
        }
    }
}
