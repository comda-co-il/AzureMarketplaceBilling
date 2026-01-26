using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CcmsCommercialPlatform.Api.Models;

/// <summary>
/// Represents a feature/token selection made by the customer during signup.
/// This will be used for metered billing later.
/// </summary>
public class FeatureSelection
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Reference to the parent marketplace subscription
    /// </summary>
    public int MarketplaceSubscriptionId { get; set; }
    
    [ForeignKey("MarketplaceSubscriptionId")]
    public MarketplaceSubscription? MarketplaceSubscription { get; set; }
    
    /// <summary>
    /// Feature identifier (e.g., "desfire", "mifare", "seos")
    /// </summary>
    [Required]
    public string FeatureId { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name of the feature
    /// </summary>
    public string FeatureName { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the feature is enabled
    /// </summary>
    public bool IsEnabled { get; set; }
    
    /// <summary>
    /// Quantity of tokens the customer wants to purchase
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Price per unit (for display purposes)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerUnit { get; set; }
    
    /// <summary>
    /// Additional notes or configuration for this feature
    /// </summary>
    public string Notes { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
