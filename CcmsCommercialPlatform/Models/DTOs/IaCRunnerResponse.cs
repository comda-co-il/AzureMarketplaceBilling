namespace CcmsCommercialPlatform.Api.Models.DTOs;

/// <summary>
/// Response from the Claude IaC Runner API after provisioning request
/// </summary>
public class IaCRunnerResponse
{
    /// <summary>
    /// Unique identifier for the provisioning job/deployment
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the provisioning request was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Human-readable message about the result
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
