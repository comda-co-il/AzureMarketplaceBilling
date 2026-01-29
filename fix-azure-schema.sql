-- SQL Script to fix MarketplaceSubscriptions table schema in Azure SQL
-- Run this script against your Azure SQL database

-- FIRST: Drop the old Country column if it exists (replaced by CountryCode/CountryOther)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'Country')
BEGIN
    ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] DROP COLUMN [Country];
    PRINT 'Dropped legacy Country column';
END
GO

-- Add WhitelistIps column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'WhitelistIps')
BEGIN
    ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [WhitelistIps] nvarchar(max) NOT NULL DEFAULT N'';
END
GO

-- Add IaCDeploymentId column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'IaCDeploymentId')
BEGIN
    ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [IaCDeploymentId] nvarchar(max) NULL;
END
GO

-- Add CcmsUrl column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'CcmsUrl')
BEGIN
    ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [CcmsUrl] nvarchar(max) NULL;
END
GO

-- Add ProvisioningMetadata column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'ProvisioningMetadata')
BEGIN
    ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [ProvisioningMetadata] nvarchar(max) NULL;
END
GO

-- Add ProvisioningErrorMessage column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'ProvisioningErrorMessage')
BEGIN
    ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [ProvisioningErrorMessage] nvarchar(max) NULL;
END
GO

-- Add ProvisioningRequestedAt column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'ProvisioningRequestedAt')
BEGIN
    ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [ProvisioningRequestedAt] datetime2 NULL;
END
GO

-- Add ProvisioningCompletedAt column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'ProvisioningCompletedAt')
BEGIN
    ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [ProvisioningCompletedAt] datetime2 NULL;
END
GO

-- Add AzureActivatedAt column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'AzureActivatedAt')
BEGIN
    ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [AzureActivatedAt] datetime2 NULL;
END
GO

-- Update EF Core migration history (table is in default schema, not CcmsCommercialPlatform)
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260129140000_AddProvisioningFields')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20260129140000_AddProvisioningFields', '8.0.0');
END
GO

PRINT 'Schema update completed successfully!';
