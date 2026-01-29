namespace CcmsCommercialPlatform.Api.Models.DTOs;

/// <summary>
/// Request payload for the Claude IaC Runner API to provision CCMS infrastructure
/// </summary>
public class IaCRunnerRequest
{
    /// <summary>
    /// Internal marketplace subscription ID
    /// </summary>
    public int SubscriptionId { get; set; }
    
    /// <summary>
    /// Azure Marketplace subscription ID
    /// </summary>
    public string AzureSubscriptionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Azure Marketplace Offer ID
    /// </summary>
    public string OfferId { get; set; } = string.Empty;
    
    /// <summary>
    /// Azure Marketplace Plan ID
    /// </summary>
    public string PlanId { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer information collected from the signup form
    /// </summary>
    public IaCRunnerCustomerInfo Customer { get; set; } = new();
    
    /// <summary>
    /// Entra ID (Azure AD) configuration for the customer's organization
    /// </summary>
    public IaCRunnerEntraConfig EntraConfig { get; set; } = new();
    
    /// <summary>
    /// Purchaser information from Azure Marketplace
    /// </summary>
    public IaCRunnerPurchaserInfo Purchaser { get; set; } = new();
    
    /// <summary>
    /// Selected features/tokens for metered billing
    /// </summary>
    public List<IaCRunnerFeature> Features { get; set; } = new();
    
    /// <summary>
    /// Timestamp of the request
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class IaCRunnerCustomerInfo
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string? CountryOther { get; set; }
    public string Comments { get; set; } = string.Empty;
}

public class IaCRunnerEntraConfig
{
    /// <summary>
    /// Application (client) ID from customer's Azure AD App Registration
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Client secret from customer's Azure AD App Registration
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// Directory (tenant) ID from customer's Azure AD
    /// </summary>
    public string TenantId { get; set; } = string.Empty;
}

public class IaCRunnerPurchaserInfo
{
    public string Email { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ObjectId { get; set; } = string.Empty;
}

public class IaCRunnerFeature
{
    public string FeatureId { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
}
