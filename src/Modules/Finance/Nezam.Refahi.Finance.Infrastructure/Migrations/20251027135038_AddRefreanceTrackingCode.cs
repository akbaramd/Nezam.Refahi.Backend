using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreanceTrackingCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReferenceType",
                schema: "finance",
                table: "Bills",
                newName: "ReferenceType");

            migrationBuilder.RenameIndex(
                name: "IX_Bills_ReferenceId_BillType",
                schema: "finance",
                table: "Bills",
                newName: "IX_Bills_ReferenceId_ReferenceType");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceTrackCode",
                schema: "finance",
                table: "Bills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "sdad");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceTrackCode",
                schema: "finance",
                table: "Bills");

            migrationBuilder.RenameColumn(
                name: "ReferenceType",
                schema: "finance",
                table: "Bills",
                newName: "ReferenceType");

            migrationBuilder.RenameIndex(
                name: "IX_Bills_ReferenceId_ReferenceType",
                schema: "finance",
                table: "Bills",
                newName: "IX_Bills_ReferenceId_BillType");
        }
    }
}
