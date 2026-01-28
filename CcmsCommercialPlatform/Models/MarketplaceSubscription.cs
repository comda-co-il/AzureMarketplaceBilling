using System.ComponentModel.DataAnnotations;
using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Models;

/// <summary>
/// Represents a subscription created from Azure Marketplace.
/// Stores all data from token resolution and customer info collection.
/// </summary>
public class MarketplaceSubscription
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Azure Marketplace subscription ID (from token resolution)
    /// </summary>
    [Required]
    public string AzureSubscriptionId { get; set; } = string.Empty;
    
    /// <summary>
    /// The original marketplace token (for reference)
    /// </summary>
    public string MarketplaceToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Offer ID from Azure Marketplace
    /// </summary>
    public string OfferId { get; set; } = string.Empty;
    
    /// <summary>
    /// Plan ID selected by customer
    /// </summary>
    public string PlanId { get; set; } = string.Empty;
    
    /// <summary>
    /// Subscription name from Azure
    /// </summary>
    public string SubscriptionName { get; set; } = string.Empty;
    
    // Purchaser information from Azure
    public string PurchaserEmail { get; set; } = string.Empty;
    public string PurchaserTenantId { get; set; } = string.Empty;
    public string PurchaserObjectId { get; set; } = string.Empty;
    
    // Beneficiary information from Azure
    public string BeneficiaryEmail { get; set; } = string.Empty;
    public string BeneficiaryTenantId { get; set; } = string.Empty;
    public string BeneficiaryObjectId { get; set; } = string.Empty;
    
    // Customer-provided information (Stage 4)
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string JobTitle { get; set; } = string.Empty;
    
    public string CountryCode { get; set; } = string.Empty;
    
    public string? CountryOther { get; set; }
    
    public string Comments { get; set; } = string.Empty;
    
    // Entra ID (Azure AD) Configuration - Customer's organization settings
    /// <summary>
    /// Application (client) ID from customer's Azure AD App Registration
    /// </summary>
    public string EntraClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Client secret from customer's Azure AD App Registration
    /// </summary>
    public string EntraClientSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// Directory (tenant) ID from customer's Azure AD
    /// </summary>
    public string EntraTenantId { get; set; } = string.Empty;
    
    // Status tracking
    public MarketplaceSubscriptionStatus Status { get; set; } = MarketplaceSubscriptionStatus.PendingCustomerInfo;
    
    // Timestamps
    public DateTime TokenResolvedAt { get; set; }
    public DateTime? CustomerInfoSubmittedAt { get; set; }
    public DateTime? FeatureSelectionCompletedAt { get; set; }
    public DateTime? SubmittedToExternalSystemAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property for feature selections
    public List<FeatureSelection> FeatureSelections { get; set; } = new();
}
