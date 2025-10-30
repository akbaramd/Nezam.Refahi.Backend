using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Facilities.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FacilityRejectionMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RejectionId",
                schema: "facilities",
                table: "FacilityRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FacilityRejections",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RejectedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RejectedByUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RejectionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityRejections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityRejections_FacilityRequests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "facilities",
                        principalTable: "FacilityRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_RejectionId",
                schema: "facilities",
                table: "FacilityRequests",
                column: "RejectionId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRejections_RejectedAt",
                schema: "facilities",
                table: "FacilityRejections",
                column: "RejectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRejections_RejectedByUserId",
                schema: "facilities",
                table: "FacilityRejections",
                column: "RejectedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRejections_RequestId",
                schema: "facilities",
                table: "FacilityRejections",
                column: "RequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityRequests_FacilityRejections_RejectionId",
                schema: "facilities",
                table: "FacilityRequests",
                column: "RejectionId",
                principalSchema: "facilities",
                principalTable: "FacilityRejections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityRequests_FacilityRejections_RejectionId",
                schema: "facilities",
                table: "FacilityRequests");

            migrationBuilder.DropTable(
                name: "FacilityRejections",
                schema: "facilities");

            migrationBuilder.DropIndex(
                name: "IX_FacilityRequests_RejectionId",
                schema: "facilities",
                table: "FacilityRequests");

            migrationBuilder.DropColumn(
                name: "RejectionId",
                schema: "facilities",
                table: "FacilityRequests");
        }
    }
}
