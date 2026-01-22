using System.ComponentModel.DataAnnotations;

namespace CcmsCommercialPlatform.Api.Models.DTOs;

public class ChangePlanRequest
{
    [Required]
    public string NewPlanId { get; set; } = string.Empty;
}
