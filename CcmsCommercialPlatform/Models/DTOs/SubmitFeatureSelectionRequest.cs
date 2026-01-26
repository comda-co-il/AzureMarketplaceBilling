using System.ComponentModel.DataAnnotations;

namespace CcmsCommercialPlatform.Api.Models.DTOs;

/// <summary>
/// Request to submit feature/token selections (Metered Billing stage)
/// </summary>
public class SubmitFeatureSelectionRequest
{
    [Required]
    public int MarketplaceSubscriptionId { get; set; }
    
    public List<FeatureSelectionItem> Features { get; set; } = new();
}

public class FeatureSelectionItem
{
    [Required]
    public string FeatureId { get; set; } = string.Empty;
    
    public string FeatureName { get; set; } = string.Empty;
    
    public bool IsEnabled { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal PricePerUnit { get; set; }
    
    public string Notes { get; set; } = string.Empty;
}
