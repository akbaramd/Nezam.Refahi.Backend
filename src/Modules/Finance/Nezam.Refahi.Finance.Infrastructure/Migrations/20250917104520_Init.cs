using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "finance");

            migrationBuilder.CreateTable(
                name: "Bills",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BillType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserNationalNumber = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    UserFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalAmountRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmountRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmountRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FullyPaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillItems",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnitPriceRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    LineTotalRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillItems_Bills_BillId",
                        column: x => x.BillId,
                        principalSchema: "finance",
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AmountRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Gateway = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatewayReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CallbackUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bills_BillId",
                        column: x => x.BillId,
                        principalSchema: "finance",
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequestedByNationalNumber = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GatewayRefundId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatewayReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProcessorNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refunds_Bills_BillId",
                        column: x => x.BillId,
                        principalSchema: "finance",
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatewayReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GatewayResponse = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAmountRials = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "finance",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillItems_BillId",
                schema: "finance",
                table: "BillItems",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_BillItems_CreatedAt",
                schema: "finance",
                table: "BillItems",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_CreatedAt",
                schema: "finance",
                table: "Bills",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_CreatedBy",
                schema: "finance",
                table: "Bills",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_DeletedAt",
                schema: "finance",
                table: "Bills",
                column: "DeletedAt",
                filter: "[DeletedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_DeletedBy",
                schema: "finance",
                table: "Bills",
                column: "DeletedBy",
                filter: "[DeletedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_IsDeleted",
                schema: "finance",
                table: "Bills",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_LastModifiedAt",
                schema: "finance",
                table: "Bills",
                column: "LastModifiedAt",
                filter: "[LastModifiedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_LastModifiedBy",
                schema: "finance",
                table: "Bills",
                column: "LastModifiedBy",
                filter: "[LastModifiedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_BillNumber",
                schema: "finance",
                table: "Bills",
                column: "BillNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bills_DueDate",
                schema: "finance",
                table: "Bills",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_IssueDate",
                schema: "finance",
                table: "Bills",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_ReferenceId_BillType",
                schema: "finance",
                table: "Bills",
                columns: new[] { "ReferenceId", "BillType" });

            migrationBuilder.CreateIndex(
                name: "IX_Bills_Status",
                schema: "finance",
                table: "Bills",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_UserNationalNumber",
                schema: "finance",
                table: "Bills",
                column: "UserNationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillId",
                schema: "finance",
                table: "Payments",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillNumber",
                schema: "finance",
                table: "Payments",
                column: "BillNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CompletedAt",
                schema: "finance",
                table: "Payments",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedAt",
                schema: "finance",
                table: "Payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_GatewayTransactionId",
                schema: "finance",
                table: "Payments",
                column: "GatewayTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                schema: "finance",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_CreatedAt",
                schema: "finance",
                table: "PaymentTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_GatewayTransactionId",
                schema: "finance",
                table: "PaymentTransactions",
                column: "GatewayTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentId",
                schema: "finance",
                table: "PaymentTransactions",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Status",
                schema: "finance",
                table: "PaymentTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_BillId",
                schema: "finance",
                table: "Refunds",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_CompletedAt",
                schema: "finance",
                table: "Refunds",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_GatewayRefundId",
                schema: "finance",
                table: "Refunds",
                column: "GatewayRefundId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_RequestedAt",
                schema: "finance",
                table: "Refunds",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_RequestedByNationalNumber",
                schema: "finance",
                table: "Refunds",
                column: "RequestedByNationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_Status",
                schema: "finance",
                table: "Refunds",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillItems",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "PaymentTransactions",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "Refunds",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "Bills",
                schema: "finance");
        }
    }
}
