using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Shared.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "shared",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxRetries = table.Column<int>(type: "int", nullable: false, defaultValue: 3)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_OccurredOn",
                schema: "shared",
                table: "OutboxMessages",
                column: "OccurredOn");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedOn",
                schema: "shared",
                table: "OutboxMessages",
                column: "ProcessedOn");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedOn_RetryCount",
                schema: "shared",
                table: "OutboxMessages",
                columns: new[] { "ProcessedOn", "RetryCount" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Type",
                schema: "shared",
                table: "OutboxMessages",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "shared");
        }
    }
}
