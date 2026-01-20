using System.ComponentModel.DataAnnotations;

namespace AzureMarketplaceBilling.Api.Models.DTOs;

public class ChangePlanRequest
{
    [Required]
    public string NewPlanId { get; set; } = string.Empty;
}
