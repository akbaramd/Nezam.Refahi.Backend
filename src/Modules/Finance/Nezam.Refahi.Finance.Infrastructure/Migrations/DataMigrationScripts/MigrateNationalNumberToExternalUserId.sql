-- =====================================================
-- Data Migration Script: NationalNumber → ExternalUserId
-- =====================================================
-- This script handles the migration from NationalNumber to ExternalUserId
-- Run this BEFORE applying the schema migration

-- Step 1: Create a temporary mapping table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserMapping' AND schema_id = SCHEMA_ID('Finance'))
BEGIN
    CREATE TABLE [Finance].[UserMapping] (
        [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [NationalNumber] NVARCHAR(20) NOT NULL,
        [ExternalUserId] UNIQUEIDENTIFIER NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsProcessed] BIT NOT NULL DEFAULT 0
    );
    
    CREATE UNIQUE INDEX [IX_UserMapping_NationalNumber] 
    ON [Finance].[UserMapping] ([NationalNumber]);
    
    CREATE UNIQUE INDEX [IX_UserMapping_ExternalUserId] 
    ON [Finance].[UserMapping] ([ExternalUserId]);
END

-- Step 2: Generate ExternalUserId for existing NationalNumbers
-- This creates a mapping between existing NationalNumbers and new ExternalUserIds
INSERT INTO [Finance].[UserMapping] ([NationalNumber], [ExternalUserId])
SELECT DISTINCT 
    [NationalNumber],
    NEWID() as [ExternalUserId]
FROM [Finance].[Wallets]
WHERE [NationalNumber] IS NOT NULL 
  AND [NationalNumber] != ''
  AND NOT EXISTS (
      SELECT 1 FROM [Finance].[UserMapping] um 
      WHERE um.[NationalNumber] = [Wallets].[NationalNumber]
  );

-- Step 3: Update Wallets table with ExternalUserId
UPDATE w 
SET [ExternalUserId] = um.[ExternalUserId]
FROM [Finance].[Wallets] w
INNER JOIN [Finance].[UserMapping] um ON w.[NationalNumber] = um.[NationalNumber]
WHERE w.[ExternalUserId] IS NULL OR w.[ExternalUserId] = '00000000-0000-0000-0000-000000000000';

-- Step 4: Update WalletSnapshots table
UPDATE ws 
SET [ExternalUserId] = um.[ExternalUserId]
FROM [Finance].[WalletSnapshots] ws
INNER JOIN [Finance].[UserMapping] um ON ws.[UserNationalNumber] = um.[NationalNumber]
WHERE ws.[ExternalUserId] IS NULL OR ws.[ExternalUserId] = '00000000-0000-0000-0000-000000000000';

-- Step 5: Update Bills table
UPDATE b 
SET [ExternalUserId] = um.[ExternalUserId]
FROM [Finance].[Bills] b
INNER JOIN [Finance].[UserMapping] um ON b.[UserNationalNumber] = um.[NationalNumber]
WHERE b.[ExternalUserId] IS NULL OR b.[ExternalUserId] = '00000000-0000-0000-0000-000000000000';

-- Step 6: Update Refunds table
UPDATE r 
SET [RequestedByExternalUserId] = um.[ExternalUserId]
FROM [Finance].[Refunds] r
INNER JOIN [Finance].[UserMapping] um ON r.[RequestedByNationalNumber] = um.[NationalNumber]
WHERE r.[RequestedByExternalUserId] IS NULL OR r.[RequestedByExternalUserId] = '00000000-0000-0000-0000-000000000000';

-- Step 7: Verify data integrity
SELECT 
    'Wallets' as TableName,
    COUNT(*) as TotalRecords,
    COUNT([ExternalUserId]) as RecordsWithExternalUserId,
    COUNT(*) - COUNT([ExternalUserId]) as RecordsWithoutExternalUserId
FROM [Finance].[Wallets]

UNION ALL

SELECT 
    'WalletSnapshots' as TableName,
    COUNT(*) as TotalRecords,
    COUNT([ExternalUserId]) as RecordsWithExternalUserId,
    COUNT(*) - COUNT([ExternalUserId]) as RecordsWithoutExternalUserId
FROM [Finance].[WalletSnapshots]

UNION ALL

SELECT 
    'Bills' as TableName,
    COUNT(*) as TotalRecords,
    COUNT([ExternalUserId]) as RecordsWithExternalUserId,
    COUNT(*) - COUNT([ExternalUserId]) as RecordsWithoutExternalUserId
FROM [Finance].[Bills]

UNION ALL

SELECT 
    'Refunds' as TableName,
    COUNT(*) as TotalRecords,
    COUNT([RequestedByExternalUserId]) as RecordsWithExternalUserId,
    COUNT(*) - COUNT([RequestedByExternalUserId]) as RecordsWithoutExternalUserId
FROM [Finance].[Refunds];

-- Step 8: Mark mapping as processed
UPDATE [Finance].[UserMapping] 
SET [IsProcessed] = 1;

PRINT 'Data migration completed successfully!';
PRINT 'Please review the verification results above before proceeding with schema migration.';
