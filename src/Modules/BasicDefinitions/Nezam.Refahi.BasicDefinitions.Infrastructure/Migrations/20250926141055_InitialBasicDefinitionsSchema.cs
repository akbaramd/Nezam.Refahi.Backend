using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialBasicDefinitionsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "definitions");

            migrationBuilder.CreateTable(
                name: "RepresentativeOffices",
                schema: "definitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExternalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManagerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EstablishedDate = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_RepresentativeOffices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepresentativeOffices_Code",
                schema: "definitions",
                table: "RepresentativeOffices",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepresentativeOffices_ExternalCode",
                schema: "definitions",
                table: "RepresentativeOffices",
                column: "ExternalCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepresentativeOffices_IsActive",
                schema: "definitions",
                table: "RepresentativeOffices",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RepresentativeOffices_ManagerName",
                schema: "definitions",
                table: "RepresentativeOffices",
                column: "ManagerName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepresentativeOffices",
                schema: "definitions");
        }
    }
}
