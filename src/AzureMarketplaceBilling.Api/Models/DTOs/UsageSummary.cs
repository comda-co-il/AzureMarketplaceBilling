using AzureMarketplaceBilling.Api.Models.Enums;

namespace AzureMarketplaceBilling.Api.Models.DTOs;

public class UsageSummary
{
    public string SubscriptionId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateTime BillingPeriodStart { get; set; }
    public DateTime BillingPeriodEnd { get; set; }
    public List<DimensionUsage> Dimensions { get; set; } = new();
    public decimal TotalOverageCharges { get; set; }
}

public class DimensionUsage
{
    public TokenUsageType DimensionType { get; set; }
    public string DimensionId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int IncludedQuantity { get; set; }
    public int UsedQuantity { get; set; }
    public int OverageQuantity { get; set; }
    public decimal OveragePrice { get; set; }
    public decimal OverageCharges { get; set; }
    public double UsagePercentage { get; set; }
    public string Status { get; set; } = "Normal"; // Normal, Warning, Critical
}
