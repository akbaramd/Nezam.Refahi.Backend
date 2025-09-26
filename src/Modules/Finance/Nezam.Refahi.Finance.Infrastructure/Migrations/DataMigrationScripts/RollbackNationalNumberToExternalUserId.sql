-- =====================================================
-- Rollback Script: ExternalUserId → NationalNumber
-- =====================================================
-- This script rolls back the migration if needed

-- Step 1: Restore NationalNumber from mapping table
UPDATE w 
SET [NationalNumber] = um.[NationalNumber]
FROM [Finance].[Wallets] w
INNER JOIN [Finance].[UserMapping] um ON w.[ExternalUserId] = um.[ExternalUserId];

UPDATE ws 
SET [UserNationalNumber] = um.[NationalNumber]
FROM [Finance].[WalletSnapshots] ws
INNER JOIN [Finance].[UserMapping] um ON ws.[ExternalUserId] = um.[ExternalUserId];

UPDATE b 
SET [UserNationalNumber] = um.[NationalNumber]
FROM [Finance].[Bills] b
INNER JOIN [Finance].[UserMapping] um ON b.[ExternalUserId] = um.[ExternalUserId];

UPDATE r 
SET [RequestedByNationalNumber] = um.[NationalNumber]
FROM [Finance].[Refunds] r
INNER JOIN [Finance].[UserMapping] um ON r.[RequestedByExternalUserId] = um.[ExternalUserId];

-- Step 2: Clear ExternalUserId fields
UPDATE [Finance].[Wallets] SET [ExternalUserId] = NULL;
UPDATE [Finance].[WalletSnapshots] SET [ExternalUserId] = NULL;
UPDATE [Finance].[Bills] SET [ExternalUserId] = NULL;
UPDATE [Finance].[Refunds] SET [RequestedByExternalUserId] = NULL;

-- Step 3: Drop mapping table (optional - keep for audit)
-- DROP TABLE [Finance].[UserMapping];

PRINT 'Rollback completed successfully!';
