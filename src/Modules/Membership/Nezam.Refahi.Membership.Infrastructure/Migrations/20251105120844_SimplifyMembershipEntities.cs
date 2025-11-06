using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyMembershipEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberFeatures_AssignedAt",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropIndex(
                name: "IX_MemberFeatures_AssignedBy",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropIndex(
                name: "IX_MemberFeatures_IsActive",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropIndex(
                name: "IX_MemberFeatures_ValidFrom",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropIndex(
                name: "IX_MemberFeatures_ValidTo",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropIndex(
                name: "IX_MemberCapabilities_AssignedAt",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropIndex(
                name: "IX_MemberCapabilities_IsActive",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropIndex(
                name: "IX_MemberCapabilities_Member_Capability_Active",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropIndex(
                name: "IX_MemberCapabilities_ValidityPeriod",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropIndex(
                name: "IX_MemberAgencies_AccessLevel",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropIndex(
                name: "IX_MemberAgencies_AssignedAt",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropIndex(
                name: "IX_MemberAgencies_IsActive",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropIndex(
                name: "IX_MemberAgencies_Member_Office_Active",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropIndex(
                name: "IX_MemberAgencies_ValidityPeriod",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropColumn(
                name: "FeatureTitle",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "membership",
                table: "MemberFeatures");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropColumn(
                name: "CapabilityTitle",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropColumn(
                name: "AccessLevel",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropColumn(
                name: "OfficeCode",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropColumn(
                name: "OfficeTitle",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCapabilities_CapabilityCapabilityKey",
                schema: "membership",
                table: "MemberCapabilities",
                newName: "IX_MemberCapabilities_CapabilityKey");

            migrationBuilder.AlterColumn<string>(
                name: "CapabilityKey",
                schema: "membership",
                table: "MemberCapabilities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_MemberId_CapabilityKey",
                schema: "membership",
                table: "MemberCapabilities",
                columns: new[] { "MemberId", "CapabilityKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_MemberId_AgencyId",
                schema: "membership",
                table: "MemberAgencies",
                columns: new[] { "MemberId", "AgencyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberCapabilities_MemberId_CapabilityKey",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropIndex(
                name: "IX_MemberAgencies_MemberId_AgencyId",
                schema: "membership",
                table: "MemberAgencies");

            migrationBuilder.RenameIndex(
                name: "IX_MemberCapabilities_CapabilityKey",
                schema: "membership",
                table: "MemberCapabilities",
                newName: "IX_MemberCapabilities_CapabilityCapabilityKey");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                schema: "membership",
                table: "MemberFeatures",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AssignedBy",
                schema: "membership",
                table: "MemberFeatures",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureTitle",
                schema: "membership",
                table: "MemberFeatures",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "membership",
                table: "MemberFeatures",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "membership",
                table: "MemberFeatures",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "membership",
                table: "MemberFeatures",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "membership",
                table: "MemberFeatures",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CapabilityKey",
                schema: "membership",
                table: "MemberCapabilities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                schema: "membership",
                table: "MemberCapabilities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AssignedBy",
                schema: "membership",
                table: "MemberCapabilities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapabilityTitle",
                schema: "membership",
                table: "MemberCapabilities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "membership",
                table: "MemberCapabilities",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "membership",
                table: "MemberCapabilities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "membership",
                table: "MemberCapabilities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "membership",
                table: "MemberCapabilities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessLevel",
                schema: "membership",
                table: "MemberAgencies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                schema: "membership",
                table: "MemberAgencies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AssignedBy",
                schema: "membership",
                table: "MemberAgencies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "membership",
                table: "MemberAgencies",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "membership",
                table: "MemberAgencies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficeCode",
                schema: "membership",
                table: "MemberAgencies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OfficeTitle",
                schema: "membership",
                table: "MemberAgencies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                schema: "membership",
                table: "MemberAgencies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                schema: "membership",
                table: "MemberAgencies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberFeatures_AssignedAt",
                schema: "membership",
                table: "MemberFeatures",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MemberFeatures_AssignedBy",
                schema: "membership",
                table: "MemberFeatures",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MemberFeatures_IsActive",
                schema: "membership",
                table: "MemberFeatures",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MemberFeatures_ValidFrom",
                schema: "membership",
                table: "MemberFeatures",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_MemberFeatures_ValidTo",
                schema: "membership",
                table: "MemberFeatures",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_AssignedAt",
                schema: "membership",
                table: "MemberCapabilities",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_IsActive",
                schema: "membership",
                table: "MemberCapabilities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_Member_Capability_Active",
                schema: "membership",
                table: "MemberCapabilities",
                columns: new[] { "MemberId", "CapabilityKey" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_ValidityPeriod",
                schema: "membership",
                table: "MemberCapabilities",
                columns: new[] { "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_AccessLevel",
                schema: "membership",
                table: "MemberAgencies",
                column: "AccessLevel");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_AssignedAt",
                schema: "membership",
                table: "MemberAgencies",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_IsActive",
                schema: "membership",
                table: "MemberAgencies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_Member_Office_Active",
                schema: "membership",
                table: "MemberAgencies",
                columns: new[] { "MemberId", "AgencyId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_ValidityPeriod",
                schema: "membership",
                table: "MemberAgencies",
                columns: new[] { "ValidFrom", "ValidTo" });
        }
    }
}
