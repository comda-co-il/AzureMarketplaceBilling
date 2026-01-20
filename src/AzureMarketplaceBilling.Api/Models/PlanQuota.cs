using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AzureMarketplaceBilling.Api.Models.Enums;

namespace AzureMarketplaceBilling.Api.Models;

public class PlanQuota
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string PlanId { get; set; } = string.Empty;
    
    [JsonIgnore]
    [ForeignKey("PlanId")]
    public Plan? Plan { get; set; }
    
    public TokenUsageType DimensionType { get; set; }
    
    public string DimensionId { get; set; } = string.Empty;  // "pki_issued", "fido_enrolled", etc.
    
    public string DisplayName { get; set; } = string.Empty;  // "PKI Certificates", "FIDO Enrollments"
    
    public int IncludedQuantity { get; set; }  // 100, 500, 2000
    
    public decimal OveragePrice { get; set; }  // $3.50, $2.50, etc.
}
