using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AzureMarketplaceBilling.Api.Models.Enums;

namespace AzureMarketplaceBilling.Api.Models;

public class Subscription
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    
    [Required]
    public string PlanId { get; set; } = string.Empty;
    
    [ForeignKey("PlanId")]
    public Plan? Plan { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime BillingPeriodStart { get; set; }
    
    public DateTime BillingPeriodEnd { get; set; }
    
    public List<UsageRecord> UsageRecords { get; set; } = new();
}
