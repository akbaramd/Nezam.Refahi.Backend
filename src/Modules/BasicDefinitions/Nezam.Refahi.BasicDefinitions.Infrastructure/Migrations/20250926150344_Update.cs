using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Capabilities",
                schema: "definitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capabilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "definitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CapabilityFeatures",
                schema: "definitions",
                columns: table => new
                {
                    CapabilityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapabilityFeatures", x => new { x.CapabilityId, x.ClaimTypeId });
                    table.ForeignKey(
                        name: "FK_CapabilityFeatures_Capabilities_CapabilityId",
                        column: x => x.CapabilityId,
                        principalSchema: "definitions",
                        principalTable: "Capabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CapabilityFeatures_Features_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalSchema: "definitions",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Capabilities_IsActive",
                schema: "definitions",
                table: "Capabilities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Capabilities_Name",
                schema: "definitions",
                table: "Capabilities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Capabilities_ValidityPeriod",
                schema: "definitions",
                table: "Capabilities",
                columns: new[] { "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_CapabilityFeatures_ClaimTypeId",
                schema: "definitions",
                table: "CapabilityFeatures",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_Title",
                schema: "definitions",
                table: "Features",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Features_Type",
                schema: "definitions",
                table: "Features",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Features_Type_Title",
                schema: "definitions",
                table: "Features",
                columns: new[] { "Type", "Title" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapabilityFeatures",
                schema: "definitions");

            migrationBuilder.DropTable(
                name: "Capabilities",
                schema: "definitions");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "definitions");
        }
    }
}
