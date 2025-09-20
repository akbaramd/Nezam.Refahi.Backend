using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "membership");

            migrationBuilder.CreateTable(
                name: "Capabilities",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capabilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MembershipNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SnapshotVersion = table.Column<long>(type: "bigint", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastAccessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAccessedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessCount = table.Column<long>(type: "bigint", nullable: false),
                    LastBackupAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EmployerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmployerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CapabilityFeatures",
                schema: "membership",
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
                        principalSchema: "membership",
                        principalTable: "Capabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CapabilityFeatures_Features_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalSchema: "membership",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberCapabilities",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapabilityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberCapabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberCapabilities_Capabilities_CapabilityId",
                        column: x => x.CapabilityId,
                        principalSchema: "membership",
                        principalTable: "Capabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberCapabilities_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "membership",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberRoles",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberRoles_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "membership",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "membership",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Capabilities_IsActive",
                schema: "membership",
                table: "Capabilities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Capabilities_Name",
                schema: "membership",
                table: "Capabilities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Capabilities_ValidityPeriod",
                schema: "membership",
                table: "Capabilities",
                columns: new[] { "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_CapabilityFeatures_ClaimTypeId",
                schema: "membership",
                table: "CapabilityFeatures",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_AssignedAt",
                schema: "membership",
                table: "MemberCapabilities",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_CapabilityId",
                schema: "membership",
                table: "MemberCapabilities",
                column: "CapabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_IsActive",
                schema: "membership",
                table: "MemberCapabilities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_Member_Capability_Active",
                schema: "membership",
                table: "MemberCapabilities",
                columns: new[] { "MemberId", "CapabilityId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_MemberId",
                schema: "membership",
                table: "MemberCapabilities",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberCapabilities_ValidityPeriod",
                schema: "membership",
                table: "MemberCapabilities",
                columns: new[] { "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_MemberRoles_IsActive",
                schema: "membership",
                table: "MemberRoles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRoles_Member_IsActive",
                schema: "membership",
                table: "MemberRoles",
                columns: new[] { "MemberId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_MemberRoles_Member_Role",
                schema: "membership",
                table: "MemberRoles",
                columns: new[] { "MemberId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_MemberRoles_Member_ValidTo",
                schema: "membership",
                table: "MemberRoles",
                columns: new[] { "MemberId", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_MemberRoles_MemberId",
                schema: "membership",
                table: "MemberRoles",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRoles_Role_IsActive",
                schema: "membership",
                table: "MemberRoles",
                columns: new[] { "RoleId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_MemberRoles_RoleId",
                schema: "membership",
                table: "MemberRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRoles_ValidTo",
                schema: "membership",
                table: "MemberRoles",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Email",
                schema: "membership",
                table: "Members",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_FirstName",
                schema: "membership",
                table: "Members",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Members_LastName",
                schema: "membership",
                table: "Members",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Members_NationalCode",
                schema: "membership",
                table: "Members",
                column: "NationalCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_PhoneNumber",
                schema: "membership",
                table: "Members",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_UserId",
                schema: "membership",
                table: "Members",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_EmployerCode",
                schema: "membership",
                table: "Roles",
                column: "EmployerCode");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_EmployerName",
                schema: "membership",
                table: "Roles",
                column: "EmployerName");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsActive",
                schema: "membership",
                table: "Roles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Key",
                schema: "membership",
                table: "Roles",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_SortOrder_Title",
                schema: "membership",
                table: "Roles",
                columns: new[] { "SortOrder", "Title" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapabilityFeatures",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "MemberCapabilities",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "MemberRoles",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Capabilities",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Members",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "membership");
        }
    }
}
