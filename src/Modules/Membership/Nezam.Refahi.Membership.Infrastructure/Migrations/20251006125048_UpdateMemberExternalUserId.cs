using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class UpdateMemberExternalUserId : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          // First, clean up any duplicate UserId values before creating unique index
          migrationBuilder.Sql(@"
                UPDATE [membership].[Members] 
                SET [UserId] = NEWID() 
                WHERE [UserId] IS NULL OR [UserId] = '00000000-0000-0000-0000-000000000000'
            ");
          
          // Handle any remaining duplicates
          migrationBuilder.Sql(@"
                WITH DuplicateUserIds AS (
                    SELECT [Id], [UserId], 
                           ROW_NUMBER() OVER (PARTITION BY [UserId] ORDER BY [Id]) as rn
                    FROM [membership].[Members]
                    WHERE [UserId] IS NOT NULL
                )
                UPDATE m 
                SET [UserId] = NEWID()
                FROM [membership].[Members] m
                INNER JOIN DuplicateUserIds d ON m.[Id] = d.[Id]
                WHERE d.rn > 1
            ");

          migrationBuilder.DropIndex(
              name: "IX_Members_UserId",
              schema: "membership",
              table: "Members");

          // Rename UserId column to ExternalUserId instead of dropping and adding
          migrationBuilder.RenameColumn(
              name: "UserId",
              table: "Members",
              newName: "ExternalUserId",
              schema: "membership");

          migrationBuilder.CreateIndex(
              name: "IX_Members_ExternalUserId",
              schema: "membership",
              table: "Members",
              column: "ExternalUserId",
              unique: true,
              filter: "[ExternalUserId] IS NOT NULL");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropIndex(
              name: "IX_Members_ExternalUserId",
              schema: "membership",
              table: "Members");

          // Rename ExternalUserId back to UserId
          migrationBuilder.RenameColumn(
              name: "ExternalUserId",
              table: "Members",
              newName: "UserId",
              schema: "membership");

          migrationBuilder.CreateIndex(
              name: "IX_Members_UserId",
              schema: "membership",
              table: "Members",
              column: "UserId",
              unique: true,
              filter: "[UserId] IS NOT NULL");
      }
  }
