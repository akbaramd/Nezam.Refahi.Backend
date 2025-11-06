using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Facilities.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class AddSelectedPriceOptionIdToFacilityRequest : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.AddColumn<Guid>(
              name: "SelectedPriceOptionId",
              schema: "facilities",
              table: "FacilityRequests",
              type: "uniqueidentifier",
              nullable: false,
              defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

          migrationBuilder.CreateIndex(
              name: "IX_FacilityRequests_SelectedPriceOptionId",
              schema: "facilities",
              table: "FacilityRequests",
              column: "SelectedPriceOptionId");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropIndex(
              name: "IX_FacilityRequests_SelectedPriceOptionId",
              schema: "facilities",
              table: "FacilityRequests");

          migrationBuilder.DropColumn(
              name: "SelectedPriceOptionId",
              schema: "facilities",
              table: "FacilityRequests");
      }
  }
