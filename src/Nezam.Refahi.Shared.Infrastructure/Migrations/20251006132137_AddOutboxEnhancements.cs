using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Shared.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class AddOutboxEnhancements : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.AddColumn<Guid>(
              name: "AggregateId",
              schema: "shared",
              table: "OutboxMessages",
              type: "uniqueidentifier",
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "CorrelationId",
              schema: "shared",
              table: "OutboxMessages",
              type: "nvarchar(500)",
              maxLength: 500,
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "DlqReason",
              schema: "shared",
              table: "OutboxMessages",
              type: "nvarchar(2000)",
              maxLength: 2000,
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "FailureReason",
              schema: "shared",
              table: "OutboxMessages",
              type: "nvarchar(2000)",
              maxLength: 2000,
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "IdempotencyKey",
              schema: "shared",
              table: "OutboxMessages",
              type: "nvarchar(500)",
              maxLength: 500,
              nullable: true);

          migrationBuilder.AddColumn<bool>(
              name: "IsPoisonMessage",
              schema: "shared",
              table: "OutboxMessages",
              type: "bit",
              nullable: false,
              defaultValue: false);

          migrationBuilder.AddColumn<DateTime>(
              name: "MovedToDlqAt",
              schema: "shared",
              table: "OutboxMessages",
              type: "datetime2",
              nullable: true);

          migrationBuilder.AddColumn<DateTime>(
              name: "NextRetryAt",
              schema: "shared",
              table: "OutboxMessages",
              type: "datetime2",
              nullable: true);

          migrationBuilder.AddColumn<DateTime>(
              name: "PoisonedAt",
              schema: "shared",
              table: "OutboxMessages",
              type: "datetime2",
              nullable: true);

          migrationBuilder.AddColumn<int>(
              name: "SchemaVersion",
              schema: "shared",
              table: "OutboxMessages",
              type: "int",
              nullable: false,
              defaultValue: 1);

          migrationBuilder.CreateTable(
              name: "EventIdempotencies",
              schema: "shared",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  IdempotencyKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                  AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                  IsProcessed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                  ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                  Error = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                  RetryCount = table.Column<int>(type: "int", nullable: false),
                  CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_EventIdempotencies", x => x.Id);
              });

          migrationBuilder.CreateIndex(
              name: "IX_OutboxMessages_AggregateId",
              schema: "shared",
              table: "OutboxMessages",
              column: "AggregateId",
              filter: "[AggregateId] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_OutboxMessages_AggregateId_OccurredOn",
              schema: "shared",
              table: "OutboxMessages",
              columns: new[] { "AggregateId", "OccurredOn" },
              filter: "[AggregateId] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_OutboxMessages_CorrelationId",
              schema: "shared",
              table: "OutboxMessages",
              column: "CorrelationId",
              filter: "[CorrelationId] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_OutboxMessages_CorrelationId_OccurredOn",
              schema: "shared",
              table: "OutboxMessages",
              columns: new[] { "CorrelationId", "OccurredOn" },
              filter: "[CorrelationId] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_OutboxMessages_IdempotencyKey",
              schema: "shared",
              table: "OutboxMessages",
              column: "IdempotencyKey",
              filter: "[IdempotencyKey] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_OutboxMessages_IsPoisonMessage",
              schema: "shared",
              table: "OutboxMessages",
              column: "IsPoisonMessage");

          migrationBuilder.CreateIndex(
              name: "IX_OutboxMessages_MovedToDlqAt",
              schema: "shared",
              table: "OutboxMessages",
              column: "MovedToDlqAt",
              filter: "[MovedToDlqAt] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_OutboxMessages_NextRetryAt",
              schema: "shared",
              table: "OutboxMessages",
              column: "NextRetryAt",
              filter: "[NextRetryAt] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_OutboxMessages_Type_ProcessedOn_RetryCount",
              schema: "shared",
              table: "OutboxMessages",
              columns: new[] { "Type", "ProcessedOn", "RetryCount" });

          migrationBuilder.CreateIndex(
              name: "IX_EventIdempotencies_AggregateId",
              schema: "shared",
              table: "EventIdempotencies",
              column: "AggregateId",
              filter: "[AggregateId] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_EventIdempotencies_IdempotencyKey",
              schema: "shared",
              table: "EventIdempotencies",
              column: "IdempotencyKey",
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_EventIdempotencies_IsProcessed",
              schema: "shared",
              table: "EventIdempotencies",
              column: "IsProcessed");

          migrationBuilder.CreateIndex(
              name: "IX_EventIdempotencies_ProcessedAt",
              schema: "shared",
              table: "EventIdempotencies",
              column: "ProcessedAt");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropTable(
              name: "EventIdempotencies",
              schema: "shared");

          migrationBuilder.DropIndex(
              name: "IX_OutboxMessages_AggregateId",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropIndex(
              name: "IX_OutboxMessages_AggregateId_OccurredOn",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropIndex(
              name: "IX_OutboxMessages_CorrelationId",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropIndex(
              name: "IX_OutboxMessages_CorrelationId_OccurredOn",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropIndex(
              name: "IX_OutboxMessages_IdempotencyKey",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropIndex(
              name: "IX_OutboxMessages_IsPoisonMessage",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropIndex(
              name: "IX_OutboxMessages_MovedToDlqAt",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropIndex(
              name: "IX_OutboxMessages_NextRetryAt",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropIndex(
              name: "IX_OutboxMessages_Type_ProcessedOn_RetryCount",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "AggregateId",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "CorrelationId",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "DlqReason",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "FailureReason",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "IdempotencyKey",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "IsPoisonMessage",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "MovedToDlqAt",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "NextRetryAt",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "PoisonedAt",
              schema: "shared",
              table: "OutboxMessages");

          migrationBuilder.DropColumn(
              name: "SchemaVersion",
              schema: "shared",
              table: "OutboxMessages");
      }
  }
