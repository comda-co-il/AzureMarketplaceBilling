namespace CcmsCommercialPlatform.Api.Models.DTOs;

public class AdminDashboardStats
{
    public int TotalActiveSubscriptions { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public decimal TotalMeteredRevenue { get; set; }
    public int UsageEventsToday { get; set; }
    public Dictionary<string, int> SubscriptionsByPlan { get; set; } = new();
}
