using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Orchestrator.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init_Orchestrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WalletDepositSagas",
                schema: "orchestrator",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CurrentState = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ExternalUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackingCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WalletDepositId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BillNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletDepositSagas", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletDepositSagas",
                schema: "orchestrator");
        }
    }
}
