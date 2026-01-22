using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;

namespace CcmsCommercialPlatform.Api.Services;

public interface ISubscriptionService
{
    Task<Subscription> CreateSubscriptionAsync(CreateSubscriptionRequest request);
    Task<Subscription?> GetSubscriptionAsync(string id);
    Task<List<Subscription>> GetAllSubscriptionsAsync(int page = 1, int pageSize = 20, string? status = null);
    Task<Subscription?> ChangePlanAsync(string subscriptionId, string newPlanId);
    Task<bool> CancelSubscriptionAsync(string subscriptionId);
    Task<Subscription?> StartNewBillingPeriodAsync(string subscriptionId);
}
