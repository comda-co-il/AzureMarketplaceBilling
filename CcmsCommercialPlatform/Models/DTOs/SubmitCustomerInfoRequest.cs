using System.ComponentModel.DataAnnotations;

namespace CcmsCommercialPlatform.Api.Models.DTOs;

/// <summary>
/// Request to submit customer information (Stage 4)
/// </summary>
public class SubmitCustomerInfoRequest
{
    [Required]
    public int MarketplaceSubscriptionId { get; set; }
    
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
    
    // Entra ID (Azure AD) Configuration
    /// <summary>
    /// Application (client) ID from Azure AD App Registration
    /// </summary>
    [Required]
    public string EntraClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// Client secret from Azure AD App Registration
    /// </summary>
    [Required]
    public string EntraClientSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// Directory (tenant) ID from Azure AD
    /// </summary>
    [Required]
    public string EntraTenantId { get; set; } = string.Empty;
    
    /// <summary>
    /// Object ID of the Azure AD group whose members should have admin access in CCMS
    /// </summary>
    [Required]
    public string EntraAdminGroupObjectId { get; set; } = string.Empty;
    
    /// <summary>
    /// List of IP addresses/CIDR ranges to whitelist for the CCMS instance
    /// </summary>
    public List<string> WhitelistIps { get; set; } = [];
}
