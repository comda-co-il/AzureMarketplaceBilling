using System.ComponentModel.DataAnnotations;
using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Models.DTOs;

public class RecordUsageRequest
{
    [Required]
    public string SubscriptionId { get; set; } = string.Empty;
    
    [Required]
    public TokenUsageType DimensionType { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}
