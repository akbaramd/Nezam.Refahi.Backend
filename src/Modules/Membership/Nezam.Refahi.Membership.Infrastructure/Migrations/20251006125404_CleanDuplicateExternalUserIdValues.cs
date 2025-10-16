using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class CleanDuplicateExternalUserIdValues : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          // First, update all NULL ExternalUserId values to unique GUIDs
          migrationBuilder.Sql(@"
                UPDATE [membership].[Members] 
                SET [ExternalUserId] = NEWID() 
                WHERE [ExternalUserId] IS NULL OR [ExternalUserId] = '00000000-0000-0000-0000-000000000000'
            ");
          
          // Then, handle any remaining duplicates by updating them to unique values
          migrationBuilder.Sql(@"
                WITH DuplicateExternalUserIds AS (
                    SELECT [Id], [ExternalUserId], 
                           ROW_NUMBER() OVER (PARTITION BY [ExternalUserId] ORDER BY [Id]) as rn
                    FROM [membership].[Members]
                    WHERE [ExternalUserId] IS NOT NULL
                )
                UPDATE m 
                SET [ExternalUserId] = NEWID()
                FROM [membership].[Members] m
                INNER JOIN DuplicateExternalUserIds d ON m.[Id] = d.[Id]
                WHERE d.rn > 1
            ");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          // This migration cannot be easily reversed as we're generating random GUIDs
          // In a real scenario, you might want to backup the data before running this migration
      }
  }
