using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AzureMarketplaceBilling.Api.Models.Enums;

namespace AzureMarketplaceBilling.Api.Models;

public class UsageRecord
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string SubscriptionId { get; set; } = string.Empty;
    
    [JsonIgnore]
    [ForeignKey("SubscriptionId")]
    public Subscription? Subscription { get; set; }
    
    public TokenUsageType DimensionType { get; set; }
    
    public string DimensionId { get; set; } = string.Empty;
    
    public DateTime BillingPeriodStart { get; set; }
    
    public int UsedQuantity { get; set; }
    
    public int ReportedOverage { get; set; }  // Already reported to Azure
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
