using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;

namespace CcmsCommercialPlatform.Api.Services;

/// <summary>
/// Demo implementation of Azure Marketplace client that logs locally instead of calling real APIs.
/// </summary>
public class DemoAzureMarketplaceClient : IAzureMarketplaceClient
{
    private readonly ILogger<DemoAzureMarketplaceClient> _logger;
    
    public DemoAzureMarketplaceClient(ILogger<DemoAzureMarketplaceClient> logger)
    {
        _logger = logger;
    }
    
    public Task<ResolvedSubscriptionInfo> ResolveTokenAsync(string token)
    {
        _logger.LogInformation(
            "[DEMO] Resolving Azure Marketplace token: {TokenPreview}...",
            token.Length > 50 ? token[..50] : token);
        
        // Generate a deterministic subscription ID based on the token
        // This ensures the same token always resolves to the same subscription
        var tokenHash = token.GetHashCode();
        var subscriptionId = $"demo-sub-{Math.Abs(tokenHash):X8}";
        
        var result = new ResolvedSubscriptionInfo
        {
            SubscriptionId = subscriptionId,
            SubscriptionName = $"Demo Subscription {Math.Abs(tokenHash) % 1000}",
            OfferId = "comsigntrust-cms",
            PlanId = "professional",
            Purchaser = new PurchaserInfo
            {
                EmailId = "purchaser@demo-company.com",
                TenantId = Guid.NewGuid().ToString(),
                ObjectId = Guid.NewGuid().ToString()
            },
            Beneficiary = new BeneficiaryInfo
            {
                EmailId = "beneficiary@demo-company.com",
                TenantId = Guid.NewGuid().ToString(),
                ObjectId = Guid.NewGuid().ToString()
            }
        };
        
        _logger.LogInformation(
            "[DEMO] Token resolved to SubscriptionId={SubscriptionId}, OfferId={OfferId}, PlanId={PlanId}",
            result.SubscriptionId,
            result.OfferId,
            result.PlanId);
        
        return Task.FromResult(result);
    }
    
    public Task<bool> ReportUsageAsync(UsageEvent usageEvent)
    {
        _logger.LogInformation(
            "[DEMO] Would report usage to Azure Metered Billing API: " +
            "ResourceId={ResourceId}, PlanId={PlanId}, Dimension={Dimension}, " +
            "Quantity={Quantity}, EffectiveStartTime={EffectiveStartTime}",
            usageEvent.ResourceId,
            usageEvent.PlanId,
            usageEvent.Dimension,
            usageEvent.Quantity,
            usageEvent.EffectiveStartTime);
        
        // Simulate successful API call
        return Task.FromResult(true);
    }
    
    public Task<bool> ActivateSubscriptionAsync(string subscriptionId, string planId)
    {
        _logger.LogInformation(
            "[DEMO] Would activate subscription via Azure Marketplace API: " +
            "SubscriptionId={SubscriptionId}, PlanId={PlanId}",
            subscriptionId,
            planId);
        
        return Task.FromResult(true);
    }
    
    public Task<bool> UpdateSubscriptionAsync(string subscriptionId, string planId)
    {
        _logger.LogInformation(
            "[DEMO] Would update subscription via Azure Marketplace API: " +
            "SubscriptionId={SubscriptionId}, NewPlanId={PlanId}",
            subscriptionId,
            planId);
        
        return Task.FromResult(true);
    }
    
    public Task<bool> CancelSubscriptionAsync(string subscriptionId)
    {
        _logger.LogInformation(
            "[DEMO] Would cancel subscription via Azure Marketplace API: " +
            "SubscriptionId={SubscriptionId}",
            subscriptionId);
        
        return Task.FromResult(true);
    }
}
