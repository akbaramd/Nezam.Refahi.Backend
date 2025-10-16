using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTomemerbCapabilityKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CapabilityName",
                schema: "membership",
                table: "MemberCapabilities",
                newName: "CapabilityTitle");

            migrationBuilder.RenameColumn(
                name: "CapabilityId",
                schema: "membership",
                table: "MemberCapabilities",
                newName: "CapabilityKey");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCapabilities_CapabilityId",
                schema: "membership",
                table: "MemberCapabilities",
                newName: "IX_MemberCapabilities_CapabilityCapabilityKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CapabilityTitle",
                schema: "membership",
                table: "MemberCapabilities",
                newName: "CapabilityName");

            migrationBuilder.RenameColumn(
                name: "CapabilityKey",
                schema: "membership",
                table: "MemberCapabilities",
                newName: "CapabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCapabilities_CapabilityCapabilityKey",
                schema: "membership",
                table: "MemberCapabilities",
                newName: "IX_MemberCapabilities_CapabilityId");
        }
    }
}
