using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class FixWalletSnapshotConfiguration : Migration
  {
      /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the constraint exists before dropping it
            var constraintExists = migrationBuilder.Sql(@"
                SELECT COUNT(*) FROM sys.foreign_keys 
                WHERE name = 'FK_WalletSnapshots_Wallets_WalletId1' 
                AND parent_object_id = OBJECT_ID('Finance.WalletSnapshots')");

            // Drop foreign key constraint if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys 
                          WHERE name = 'FK_WalletSnapshots_Wallets_WalletId1' 
                          AND parent_object_id = OBJECT_ID('Finance.WalletSnapshots'))
                BEGIN
                    ALTER TABLE [Finance].[WalletSnapshots] DROP CONSTRAINT [FK_WalletSnapshots_Wallets_WalletId1]
                END");

            // Drop index if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes 
                          WHERE name = 'IX_WalletSnapshots_WalletId1' 
                          AND object_id = OBJECT_ID('Finance.WalletSnapshots'))
                BEGIN
                    DROP INDEX [IX_WalletSnapshots_WalletId1] ON [Finance].[WalletSnapshots]
                END");

            // Drop column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns 
                          WHERE name = 'WalletId1' 
                          AND object_id = OBJECT_ID('Finance.WalletSnapshots'))
                BEGIN
                    ALTER TABLE [Finance].[WalletSnapshots] DROP COLUMN [WalletId1]
                END");

            // Rename index if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes 
                          WHERE name = 'IX_WalletSnapshots_WalletId_SnapshotDate' 
                          AND object_id = OBJECT_ID('Finance.WalletSnapshots'))
                BEGIN
                    EXEC sp_rename 'Finance.WalletSnapshots.IX_WalletSnapshots_WalletId_SnapshotDate', 'IX_WalletSnapshots_WalletId_SnapshotDate_Unique', 'INDEX'
                END");
        }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.RenameIndex(
              name: "IX_WalletSnapshots_WalletId_SnapshotDate_Unique",
              schema: "Finance",
              table: "WalletSnapshots",
              newName: "IX_WalletSnapshots_WalletId_SnapshotDate");

          migrationBuilder.AddColumn<Guid>(
              name: "WalletId1",
              schema: "Finance",
              table: "WalletSnapshots",
              type: "uniqueidentifier",
              nullable: true);

          migrationBuilder.CreateIndex(
              name: "IX_WalletSnapshots_WalletId1",
              schema: "Finance",
              table: "WalletSnapshots",
              column: "WalletId1");

          migrationBuilder.AddForeignKey(
              name: "FK_WalletSnapshots_Wallets_WalletId1",
              schema: "Finance",
              table: "WalletSnapshots",
              column: "WalletId1",
              principalSchema: "finance",
              principalTable: "Wallets",
              principalColumn: "Id");
      }
  }
