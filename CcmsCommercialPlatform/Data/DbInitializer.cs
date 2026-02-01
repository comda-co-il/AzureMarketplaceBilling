using Microsoft.EntityFrameworkCore;
using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Check if using SQLite (for local development)
        bool isSqlite = context.Database.ProviderName?.Contains("Sqlite") ?? false;
        
        if (isSqlite)
        {
            // For SQLite in development, handle schema changes automatically
            InitializeSqlite(context);
        }
        else
        {
            // Create schema if it doesn't exist (for SQL Server with custom schema)
            try
            {
                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'CcmsCommercialPlatform')
                    BEGIN
                        EXEC('CREATE SCHEMA [CcmsCommercialPlatform]')
                    END");
            }
            catch
            {
                // Ignore if schema already exists
            }
            
            // Apply migrations for SQL Server
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                // If migrations fail (e.g., due to SQLite-generated migrations), 
                // manually add missing columns for the provisioning fields
                Console.WriteLine($"Migration failed, attempting manual schema update: {ex.Message}");
                ApplyManualSchemaUpdates(context);
            }
        }
        
        // Check if data already exists
        try
        {
            if (context.Plans.Any())
            {
                return; // Database has been seeded
            }
        }
        catch
        {
            // Tables might not exist yet, continue with seeding
        }
        
        // Seed Plans
        var plans = new Plan[]
        {
            new Plan
            {
                Id = "starter",
                Name = "Starter",
                Description = "Perfect for small organizations getting started with credential management",
                MonthlyPrice = 299,
                IsPopular = false
            },
            new Plan
            {
                Id = "professional",
                Name = "Professional",
                Description = "Best value for growing businesses with advanced needs",
                MonthlyPrice = 799,
                IsPopular = true
            },
            new Plan
            {
                Id = "enterprise",
                Name = "Enterprise",
                Description = "For large-scale deployments requiring maximum capacity",
                MonthlyPrice = 1999,
                IsPopular = false
            }
        };
        
        context.Plans.AddRange(plans);
        context.SaveChanges();
        
        // Seed Plan Quotas
        var quotas = new List<PlanQuota>();
        
        // Define quota configurations
        var quotaConfigs = new[]
        {
            new { Type = TokenUsageType.Pki, DimensionId = "pki_issued", DisplayName = "PKI Certificates", Starter = 100, Professional = 500, Enterprise = 2000, Price = 3.50m },
            new { Type = TokenUsageType.Print, DimensionId = "print_jobs", DisplayName = "Print Jobs", Starter = 200, Professional = 1000, Enterprise = 5000, Price = 0.75m },
            new { Type = TokenUsageType.Desfire, DimensionId = "desfire_encoded", DisplayName = "DESFire Encodings", Starter = 100, Professional = 500, Enterprise = 2000, Price = 1.25m },
            new { Type = TokenUsageType.Prox, DimensionId = "prox_encoded", DisplayName = "Prox Encodings", Starter = 100, Professional = 500, Enterprise = 2000, Price = 0.35m },
            new { Type = TokenUsageType.Biometric, DimensionId = "biometric_enrolled", DisplayName = "Biometric Enrollments", Starter = 50, Professional = 200, Enterprise = 1000, Price = 3.00m },
            new { Type = TokenUsageType.Wallet, DimensionId = "wallet_provisioned", DisplayName = "Wallet Credentials", Starter = 50, Professional = 200, Enterprise = 1000, Price = 1.75m },
            new { Type = TokenUsageType.Fido, DimensionId = "fido_enrolled", DisplayName = "FIDO Enrollments", Starter = 100, Professional = 500, Enterprise = 2000, Price = 2.50m }
        };
        
        foreach (var config in quotaConfigs)
        {
            // Starter quotas
            quotas.Add(new PlanQuota
            {
                PlanId = "starter",
                DimensionType = config.Type,
                DimensionId = config.DimensionId,
                DisplayName = config.DisplayName,
                IncludedQuantity = config.Starter,
                OveragePrice = config.Price
            });
            
            // Professional quotas (10% discount on overage)
            quotas.Add(new PlanQuota
            {
                PlanId = "professional",
                DimensionType = config.Type,
                DimensionId = config.DimensionId,
                DisplayName = config.DisplayName,
                IncludedQuantity = config.Professional,
                OveragePrice = Math.Round(config.Price * 0.90m, 2)
            });
            
            // Enterprise quotas (20% discount on overage)
            quotas.Add(new PlanQuota
            {
                PlanId = "enterprise",
                DimensionType = config.Type,
                DimensionId = config.DimensionId,
                DisplayName = config.DisplayName,
                IncludedQuantity = config.Enterprise,
                OveragePrice = Math.Round(config.Price * 0.80m, 2)
            });
        }
        
        context.PlanQuotas.AddRange(quotas);
        context.SaveChanges();
    }
    
    /// <summary>
    /// Initialize SQLite database with automatic schema updates for development.
    /// If schema is outdated, drops and recreates the database.
    /// </summary>
    private static void InitializeSqlite(AppDbContext context)
    {
        bool databaseExists = context.Database.CanConnect();
        
        if (!databaseExists)
        {
            // Database doesn't exist, create it fresh
            Console.WriteLine("SQLite database doesn't exist. Creating new database...");
            context.Database.EnsureCreated();
            return;
        }
        
        // Database exists - check if schema matches current model
        bool schemaValid = ValidateSqliteSchema(context);
        
        if (!schemaValid)
        {
            Console.WriteLine("SQLite schema is outdated. Recreating database with new schema...");
            
            // Drop and recreate for development - this is acceptable for local dev
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            Console.WriteLine("SQLite database recreated successfully with updated schema.");
        }
        else
        {
            Console.WriteLine("SQLite schema is up to date.");
        }
    }
    
    /// <summary>
    /// Validates that the SQLite database schema matches the current EF model.
    /// Returns false if any expected columns are missing.
    /// </summary>
    private static bool ValidateSqliteSchema(AppDbContext context)
    {
        try
        {
            // Get expected columns from the MarketplaceSubscription entity (most frequently updated)
            List<string> expectedColumns =
            [
                "EntraClientId",
                "EntraClientSecret", 
                "EntraTenantId",
                "EntraAdminGroupObjectId",
                "WhitelistIps",
                "IaCDeploymentId",
                "CcmsUrl",
                "ProvisioningMetadata",
                "ProvisioningErrorMessage",
                "ProvisioningRequestedAt",
                "ProvisioningCompletedAt",
                "AzureActivatedAt"
            ];
            
            // Query SQLite for actual columns in MarketplaceSubscriptions table
            List<string> actualColumns = [];
            using (Microsoft.Data.Sqlite.SqliteConnection connection = new(context.Database.GetConnectionString()))
            {
                connection.Open();
                using Microsoft.Data.Sqlite.SqliteCommand command = connection.CreateCommand();
                command.CommandText = "PRAGMA table_info(MarketplaceSubscriptions);";
                using Microsoft.Data.Sqlite.SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    actualColumns.Add(reader.GetString(1)); // Column name is at index 1
                }
            }
            
            // Check if all expected columns exist
            foreach (string expectedColumn in expectedColumns)
            {
                if (!actualColumns.Contains(expectedColumn, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Missing column detected: {expectedColumn}");
                    return false;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Schema validation failed: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Manually applies schema updates for SQL Server when migrations fail
    /// (e.g., when migrations were generated for SQLite)
    /// </summary>
    private static void ApplyManualSchemaUpdates(AppDbContext context)
    {
        string[] sqlStatements =
        [
            // Add WhitelistIps column
            @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'WhitelistIps')
              ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [WhitelistIps] nvarchar(max) NOT NULL DEFAULT N''",
            
            // Add IaCDeploymentId column
            @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'IaCDeploymentId')
              ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [IaCDeploymentId] nvarchar(max) NULL",
            
            // Add CcmsUrl column
            @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'CcmsUrl')
              ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [CcmsUrl] nvarchar(max) NULL",
            
            // Add ProvisioningMetadata column
            @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'ProvisioningMetadata')
              ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [ProvisioningMetadata] nvarchar(max) NULL",
            
            // Add ProvisioningErrorMessage column
            @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'ProvisioningErrorMessage')
              ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [ProvisioningErrorMessage] nvarchar(max) NULL",
            
            // Add ProvisioningRequestedAt column
            @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'ProvisioningRequestedAt')
              ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [ProvisioningRequestedAt] datetime2 NULL",
            
            // Add ProvisioningCompletedAt column
            @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'ProvisioningCompletedAt')
              ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [ProvisioningCompletedAt] datetime2 NULL",
            
            // Add AzureActivatedAt column
            @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'AzureActivatedAt')
              ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [AzureActivatedAt] datetime2 NULL",
            
            // Add EntraAdminGroupObjectId column
            @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[CcmsCommercialPlatform].[MarketplaceSubscriptions]') AND name = 'EntraAdminGroupObjectId')
              ALTER TABLE [CcmsCommercialPlatform].[MarketplaceSubscriptions] ADD [EntraAdminGroupObjectId] nvarchar(450) NOT NULL DEFAULT N''"
        ];
        
        foreach (string sql in sqlStatements)
        {
            try
            {
                context.Database.ExecuteSqlRaw(sql);
                Console.WriteLine("Schema update applied successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Schema update warning: {ex.Message}");
                // Continue with other statements even if one fails
            }
        }
        
        Console.WriteLine("Manual schema updates completed");
    }
}
