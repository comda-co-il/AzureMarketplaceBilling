using Microsoft.EntityFrameworkCore;
using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Check if using SQLite (for local development)
        var isSqlite = context.Database.ProviderName?.Contains("Sqlite") ?? false;
        
        if (isSqlite)
        {
            // For SQLite, use EnsureCreated (migrations are SQL Server specific)
            context.Database.EnsureCreated();
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
            context.Database.Migrate();
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
}
