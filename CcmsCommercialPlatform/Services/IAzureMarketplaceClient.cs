using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;

namespace CcmsCommercialPlatform.Api.Services;

public interface IAzureMarketplaceClient
{
    /// <summary>
    /// Resolve an Azure Marketplace token to get subscription details
    /// </summary>
    Task<ResolvedSubscriptionInfo> ResolveTokenAsync(string token);
    
    Task<bool> ReportUsageAsync(UsageEvent usageEvent);
    Task<bool> ActivateSubscriptionAsync(string subscriptionId, string planId);
    Task<bool> UpdateSubscriptionAsync(string subscriptionId, string planId);
    Task<bool> CancelSubscriptionAsync(string subscriptionId);
}
