using AzureMarketplaceBilling.Api.Models;

namespace AzureMarketplaceBilling.Api.Services;

public interface IAzureMarketplaceClient
{
    Task<bool> ReportUsageAsync(UsageEvent usageEvent);
    Task<bool> ActivateSubscriptionAsync(string subscriptionId, string planId);
    Task<bool> UpdateSubscriptionAsync(string subscriptionId, string planId);
    Task<bool> CancelSubscriptionAsync(string subscriptionId);
}
