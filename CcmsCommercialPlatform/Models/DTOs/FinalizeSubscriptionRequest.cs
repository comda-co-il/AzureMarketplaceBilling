using System.ComponentModel.DataAnnotations;

namespace CcmsCommercialPlatform.Api.Models.DTOs;

/// <summary>
/// Request to finalize subscription and submit to external system
/// </summary>
public class FinalizeSubscriptionRequest
{
    [Required]
    public int MarketplaceSubscriptionId { get; set; }
}
