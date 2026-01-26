using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Models.DTOs;

/// <summary>
/// Response containing full marketplace subscription details
/// </summary>
public class MarketplaceSubscriptionResponse
{
    public int Id { get; set; }
    public string AzureSubscriptionId { get; set; } = string.Empty;
    public string OfferId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
    
    // Purchaser info
    public string PurchaserEmail { get; set; } = string.Empty;
    public string PurchaserTenantId { get; set; } = string.Empty;
    
    // Customer info
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    
    // Status
    public MarketplaceSubscriptionStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    
    // Feature selections
    public List<FeatureSelectionResponse> FeatureSelections { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? CustomerInfoSubmittedAt { get; set; }
    public DateTime? FeatureSelectionCompletedAt { get; set; }
    public DateTime? SubmittedToExternalSystemAt { get; set; }
}

public class FeatureSelectionResponse
{
    public int Id { get; set; }
    public string FeatureId { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public string Notes { get; set; } = string.Empty;
}
