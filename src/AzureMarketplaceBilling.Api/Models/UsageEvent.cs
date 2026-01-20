using System.ComponentModel.DataAnnotations;

namespace AzureMarketplaceBilling.Api.Models;

public class UsageEvent
{
    [Key]
    public int Id { get; set; }
    
    public string ResourceId { get; set; } = string.Empty;  // Subscription ID
    
    public string PlanId { get; set; } = string.Empty;
    
    public string Dimension { get; set; } = string.Empty;  // "pki_issued"
    
    public int Quantity { get; set; }  // Overage amount
    
    public decimal Amount { get; set; }  // Calculated cost
    
    public DateTime EffectiveStartTime { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } = "Pending";  // Pending, Submitted, Accepted, Rejected
}
