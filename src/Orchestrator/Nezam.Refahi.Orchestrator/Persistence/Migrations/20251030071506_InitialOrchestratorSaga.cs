using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Orchestrator.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialOrchestratorSaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReservationPaymentSagas",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CurrentState = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExternalUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAmountRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BillNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GatewayReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Gateway = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentFailedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationPaymentSagas", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationPaymentSagas");
        }
    }
}
