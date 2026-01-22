using System.ComponentModel.DataAnnotations;

namespace CcmsCommercialPlatform.Api.Models.DTOs;

public class CreateSubscriptionRequest
{
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    
    [Required]
    public string PlanId { get; set; } = string.Empty;
}
