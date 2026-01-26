using System.ComponentModel.DataAnnotations;

namespace CcmsCommercialPlatform.Api.Models.DTOs;

/// <summary>
/// Request to resolve an Azure Marketplace token
/// </summary>
public class ResolveTokenRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
}
