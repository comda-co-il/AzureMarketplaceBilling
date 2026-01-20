using Microsoft.EntityFrameworkCore;
using AzureMarketplaceBilling.Api.Data;
using AzureMarketplaceBilling.Api.Models;
using AzureMarketplaceBilling.Api.Models.DTOs;
using AzureMarketplaceBilling.Api.Models.Enums;

namespace AzureMarketplaceBilling.Api.Services;

public class MeteredBillingService : IMeteredBillingService
{
    private readonly AppDbContext _context;
    private readonly IAzureMarketplaceClient _azureClient;
    private readonly ILogger<MeteredBillingService> _logger;
    
    public MeteredBillingService(
        AppDbContext context, 
        IAzureMarketplaceClient azureClient,
        ILogger<MeteredBillingService> logger)
    {
        _context = context;
        _azureClient = azureClient;
        _logger = logger;
    }
    
    public async Task<UsageRecord> RecordUsageAsync(string subscriptionId, TokenUsageType dimensionType, int quantity)
    {
        // 1. Get subscription and its plan
        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .ThenInclude(p => p!.Quotas)
            .FirstOrDefaultAsync(s => s.Id == subscriptionId);
        
        if (subscription == null)
        {
            throw new ArgumentException($"Subscription '{subscriptionId}' not found");
        }
        
        if (subscription.Status != SubscriptionStatus.Active)
        {
            throw new InvalidOperationException($"Subscription is not active. Current status: {subscription.Status}");
        }
        
        var quota = subscription.Plan!.Quotas.FirstOrDefault(q => q.DimensionType == dimensionType);
        if (quota == null)
        {
            throw new ArgumentException($"No quota found for dimension type '{dimensionType}'");
        }
        
        // 2. Get or create usage record for current billing period
        var usage = await GetOrCreateUsageRecordAsync(subscription, dimensionType, quota.DimensionId);
        
        // 3. Add quantity to used amount
        usage.UsedQuantity += quantity;
        usage.LastUpdated = DateTime.UtcNow;
        
        // 4. Check if we need to report overage
        if (usage.UsedQuantity > quota.IncludedQuantity)
        {
            int totalOverage = usage.UsedQuantity - quota.IncludedQuantity;
            int newOverageToReport = totalOverage - usage.ReportedOverage;
            
            if (newOverageToReport > 0)
            {
                // 5. Report to Azure Metered Billing API
                var usageEvent = new UsageEvent
                {
                    ResourceId = subscriptionId,
                    PlanId = subscription.PlanId,
                    Dimension = quota.DimensionId,
                    Quantity = newOverageToReport,
                    Amount = newOverageToReport * quota.OveragePrice,
                    EffectiveStartTime = DateTime.UtcNow,
                    Status = "Pending"
                };
                
                _context.UsageEvents.Add(usageEvent);
                
                // Report to Azure (or demo log)
                var success = await _azureClient.ReportUsageAsync(usageEvent);
                usageEvent.Status = success ? "Accepted" : "Failed";
                
                usage.ReportedOverage = totalOverage;
                
                _logger.LogInformation(
                    "Reported overage for subscription {SubscriptionId}: {Quantity} {Dimension} = ${Amount}",
                    subscriptionId, newOverageToReport, quota.DimensionId, usageEvent.Amount);
            }
        }
        
        // 6. Save usage record
        await _context.SaveChangesAsync();
        
        return usage;
    }
    
    public async Task<UsageSummary> GetUsageSummaryAsync(string subscriptionId)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .ThenInclude(p => p!.Quotas)
            .FirstOrDefaultAsync(s => s.Id == subscriptionId);
        
        if (subscription == null)
        {
            throw new ArgumentException($"Subscription '{subscriptionId}' not found");
        }
        
        var usageRecords = await _context.UsageRecords
            .Where(r => r.SubscriptionId == subscriptionId && 
                        r.BillingPeriodStart == subscription.BillingPeriodStart)
            .ToListAsync();
        
        var summary = new UsageSummary
        {
            SubscriptionId = subscriptionId,
            PlanId = subscription.PlanId,
            PlanName = subscription.Plan!.Name,
            BillingPeriodStart = subscription.BillingPeriodStart,
            BillingPeriodEnd = subscription.BillingPeriodEnd,
            Dimensions = new List<DimensionUsage>()
        };
        
        foreach (var quota in subscription.Plan.Quotas)
        {
            var usageRecord = usageRecords.FirstOrDefault(r => r.DimensionType == quota.DimensionType);
            var usedQuantity = usageRecord?.UsedQuantity ?? 0;
            var overageQuantity = Math.Max(0, usedQuantity - quota.IncludedQuantity);
            var usagePercentage = quota.IncludedQuantity > 0 
                ? (double)usedQuantity / quota.IncludedQuantity * 100 
                : 0;
            
            var status = usagePercentage switch
            {
                >= 90 => "Critical",
                >= 70 => "Warning",
                _ => "Normal"
            };
            
            summary.Dimensions.Add(new DimensionUsage
            {
                DimensionType = quota.DimensionType,
                DimensionId = quota.DimensionId,
                DisplayName = quota.DisplayName,
                IncludedQuantity = quota.IncludedQuantity,
                UsedQuantity = usedQuantity,
                OverageQuantity = overageQuantity,
                OveragePrice = quota.OveragePrice,
                OverageCharges = overageQuantity * quota.OveragePrice,
                UsagePercentage = Math.Round(usagePercentage, 1),
                Status = status
            });
        }
        
        summary.TotalOverageCharges = summary.Dimensions.Sum(d => d.OverageCharges);
        
        return summary;
    }
    
    public async Task<List<UsageRecord>> GetUsageHistoryAsync(
        string subscriptionId, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        TokenUsageType? dimensionType = null)
    {
        var query = _context.UsageRecords
            .Where(r => r.SubscriptionId == subscriptionId)
            .AsQueryable();
        
        if (startDate.HasValue)
        {
            query = query.Where(r => r.BillingPeriodStart >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            query = query.Where(r => r.BillingPeriodStart <= endDate.Value);
        }
        
        if (dimensionType.HasValue)
        {
            query = query.Where(r => r.DimensionType == dimensionType.Value);
        }
        
        return await query
            .OrderByDescending(r => r.LastUpdated)
            .ToListAsync();
    }
    
    public async Task<List<UsageEvent>> GetUsageEventsAsync(string? subscriptionId = null, int page = 1, int pageSize = 50)
    {
        var query = _context.UsageEvents.AsQueryable();
        
        if (!string.IsNullOrEmpty(subscriptionId))
        {
            query = query.Where(e => e.ResourceId == subscriptionId);
        }
        
        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    private async Task<UsageRecord> GetOrCreateUsageRecordAsync(
        Subscription subscription, 
        TokenUsageType dimensionType,
        string dimensionId)
    {
        var usageRecord = await _context.UsageRecords
            .FirstOrDefaultAsync(r => 
                r.SubscriptionId == subscription.Id &&
                r.DimensionType == dimensionType &&
                r.BillingPeriodStart == subscription.BillingPeriodStart);
        
        if (usageRecord == null)
        {
            usageRecord = new UsageRecord
            {
                SubscriptionId = subscription.Id,
                DimensionType = dimensionType,
                DimensionId = dimensionId,
                BillingPeriodStart = subscription.BillingPeriodStart,
                UsedQuantity = 0,
                ReportedOverage = 0,
                LastUpdated = DateTime.UtcNow
            };
            
            _context.UsageRecords.Add(usageRecord);
        }
        
        return usageRecord;
    }
}
