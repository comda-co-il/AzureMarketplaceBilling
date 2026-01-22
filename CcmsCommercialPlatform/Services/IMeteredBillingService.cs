using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;
using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Services;

public interface IMeteredBillingService
{
    Task<UsageRecord> RecordUsageAsync(string subscriptionId, TokenUsageType dimensionType, int quantity);
    Task<UsageSummary> GetUsageSummaryAsync(string subscriptionId);
    Task<List<UsageRecord>> GetUsageHistoryAsync(string subscriptionId, DateTime? startDate = null, DateTime? endDate = null, TokenUsageType? dimensionType = null);
    Task<List<UsageEvent>> GetUsageEventsAsync(string? subscriptionId = null, int page = 1, int pageSize = 50);
}
