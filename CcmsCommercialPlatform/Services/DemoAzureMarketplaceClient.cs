using CcmsCommercialPlatform.Api.Models;

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
