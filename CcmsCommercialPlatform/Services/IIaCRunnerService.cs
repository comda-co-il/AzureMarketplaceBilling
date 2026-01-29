using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;

namespace CcmsCommercialPlatform.Api.Services;

/// <summary>
/// Service interface for Claude IaC Runner API
/// Handles automated CCMS infrastructure provisioning
/// </summary>
public interface IIaCRunnerService
{
    /// <summary>
    /// Provisions a new CCMS container for the customer
    /// </summary>
    /// <param name="subscription">The marketplace subscription with all customer details</param>
    /// <returns>Response indicating success/failure of the provisioning request</returns>
    Task<IaCRunnerResponse> ProvisionInfrastructureAsync(MarketplaceSubscription subscription);
}
