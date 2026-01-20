using AzureMarketplaceBilling.Api.Models;
using AzureMarketplaceBilling.Api.Models.DTOs;

namespace AzureMarketplaceBilling.Api.Services;

public interface ISubscriptionService
{
    Task<Subscription> CreateSubscriptionAsync(CreateSubscriptionRequest request);
    Task<Subscription?> GetSubscriptionAsync(string id);
    Task<List<Subscription>> GetAllSubscriptionsAsync(int page = 1, int pageSize = 20, string? status = null);
    Task<Subscription?> ChangePlanAsync(string subscriptionId, string newPlanId);
    Task<bool> CancelSubscriptionAsync(string subscriptionId);
    Task<Subscription?> StartNewBillingPeriodAsync(string subscriptionId);
}
