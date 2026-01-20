using AzureMarketplaceBilling.Api.Models;
using AzureMarketplaceBilling.Api.Models.DTOs;
using AzureMarketplaceBilling.Api.Models.Enums;

namespace AzureMarketplaceBilling.Api.Services;

public interface IMeteredBillingService
{
    Task<UsageRecord> RecordUsageAsync(string subscriptionId, TokenUsageType dimensionType, int quantity);
    Task<UsageSummary> GetUsageSummaryAsync(string subscriptionId);
    Task<List<UsageRecord>> GetUsageHistoryAsync(string subscriptionId, DateTime? startDate = null, DateTime? endDate = null, TokenUsageType? dimensionType = null);
    Task<List<UsageEvent>> GetUsageEventsAsync(string? subscriptionId = null, int page = 1, int pageSize = 50);
}
