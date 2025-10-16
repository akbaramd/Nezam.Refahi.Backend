using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Facilities.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class RenameExternalUserIdToMemberId : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropForeignKey(
              name: "FK_FacilityCapabilityPolicies_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityCapabilityPolicies");

          migrationBuilder.DropForeignKey(
              name: "FK_FacilityCycles_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropForeignKey(
              name: "FK_FacilityFeatures_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityFeatures");

          migrationBuilder.RenameColumn(
              name: "ExternalUserId",
              schema: "facilities",
              table: "FacilityRequests",
              newName: "MemberId");

          migrationBuilder.RenameIndex(
              name: "IX_FacilityRequests_ExternalUserId",
              schema: "facilities",
              table: "FacilityRequests",
              newName: "IX_FacilityRequests_MemberId");

          migrationBuilder.AddForeignKey(
              name: "FK_FacilityCapabilityPolicies_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityCapabilityPolicies",
              column: "FacilityId",
              principalSchema: "facilities",
              principalTable: "Facilities",
              principalColumn: "Id",
              onDelete: ReferentialAction.Restrict);

          migrationBuilder.AddForeignKey(
              name: "FK_FacilityCycles_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityCycles",
              column: "FacilityId",
              principalSchema: "facilities",
              principalTable: "Facilities",
              principalColumn: "Id",
              onDelete: ReferentialAction.Restrict);

          migrationBuilder.AddForeignKey(
              name: "FK_FacilityFeatures_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityFeatures",
              column: "FacilityId",
              principalSchema: "facilities",
              principalTable: "Facilities",
              principalColumn: "Id",
              onDelete: ReferentialAction.Restrict);
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropForeignKey(
              name: "FK_FacilityCapabilityPolicies_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityCapabilityPolicies");

          migrationBuilder.DropForeignKey(
              name: "FK_FacilityCycles_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropForeignKey(
              name: "FK_FacilityFeatures_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityFeatures");

          migrationBuilder.RenameColumn(
              name: "MemberId",
              schema: "facilities",
              table: "FacilityRequests",
              newName: "ExternalUserId");

          migrationBuilder.RenameIndex(
              name: "IX_FacilityRequests_MemberId",
              schema: "facilities",
              table: "FacilityRequests",
              newName: "IX_FacilityRequests_ExternalUserId");

          migrationBuilder.AddForeignKey(
              name: "FK_FacilityCapabilityPolicies_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityCapabilityPolicies",
              column: "FacilityId",
              principalSchema: "facilities",
              principalTable: "Facilities",
              principalColumn: "Id",
              onDelete: ReferentialAction.Restrict);

          migrationBuilder.AddForeignKey(
              name: "FK_FacilityCycles_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityCycles",
              column: "FacilityId",
              principalSchema: "facilities",
              principalTable: "Facilities",
              principalColumn: "Id",
              onDelete: ReferentialAction.Restrict);

          migrationBuilder.AddForeignKey(
              name: "FK_FacilityFeatures_Facilities_FacilityId",
              schema: "facilities",
              table: "FacilityFeatures",
              column: "FacilityId",
              principalSchema: "facilities",
              principalTable: "Facilities",
              principalColumn: "Id",
              onDelete: ReferentialAction.Restrict);
      }
  }
