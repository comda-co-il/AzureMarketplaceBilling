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
}
