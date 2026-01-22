using Microsoft.EntityFrameworkCore;
using AzureMarketplaceBilling.Api.Models;

namespace AzureMarketplaceBilling.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanQuota> PlanQuotas => Set<PlanQuota>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<UsageRecord> UsageRecords => Set<UsageRecord>();
    public DbSet<UsageEvent> UsageEvents => Set<UsageEvent>();
    
    /// <summary>
    /// Stores raw Azure webhook events for development/debugging purposes
    /// </summary>
    public DbSet<AzureWebhookEvent> AzureWebhookEvents => Set<AzureWebhookEvent>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Plan configuration
        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MonthlyPrice).HasPrecision(18, 2);
            entity.HasMany(e => e.Quotas)
                .WithOne(q => q.Plan)
                .HasForeignKey(q => q.PlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // PlanQuota configuration
        modelBuilder.Entity<PlanQuota>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OveragePrice).HasPrecision(18, 2);
        });
        
        // Subscription configuration
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Plan)
                .WithMany()
                .HasForeignKey(e => e.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.UsageRecords)
                .WithOne(u => u.Subscription)
                .HasForeignKey(u => u.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // UsageRecord configuration
        modelBuilder.Entity<UsageRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SubscriptionId, e.DimensionType, e.BillingPeriodStart });
        });
        
        // UsageEvent configuration
        modelBuilder.Entity<UsageEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasIndex(e => e.ResourceId);
            entity.HasIndex(e => e.CreatedAt);
        });
        
        // AzureWebhookEvent configuration (for development/debugging)
        modelBuilder.Entity<AzureWebhookEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ReceivedAt);
            entity.HasIndex(e => e.SubscriptionId);
            entity.HasIndex(e => e.Action);
        });
    }
}
