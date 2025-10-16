using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdDDeatruesForMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberFeatures",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FeatureTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberFeatures_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "membership",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_MemberFeatures_FeatureKey",
                schema: "membership",
                table: "MemberFeatures",
                column: "FeatureKey");

            migrationBuilder.CreateIndex(
                name: "IX_MemberFeatures_IsActive",
                schema: "membership",
                table: "MemberFeatures",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MemberFeatures_MemberId",
                schema: "membership",
                table: "MemberFeatures",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberFeatures_MemberId_FeatureKey",
                schema: "membership",
                table: "MemberFeatures",
                columns: new[] { "MemberId", "FeatureKey" },
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberFeatures",
                schema: "membership");
        }
    }
}
