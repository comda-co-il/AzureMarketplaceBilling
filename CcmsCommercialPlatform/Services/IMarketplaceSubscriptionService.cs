using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;

namespace CcmsCommercialPlatform.Api.Services;

public interface IMarketplaceSubscriptionService
{
    /// <summary>
    /// Resolve token and create a new marketplace subscription record
    /// </summary>
    Task<ResolvedSubscriptionInfo> ResolveTokenAndCreateAsync(string token);
    
    /// <summary>
    /// Get a marketplace subscription by ID
    /// </summary>
    Task<MarketplaceSubscriptionResponse?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get a marketplace subscription by Azure subscription ID
    /// </summary>
    Task<MarketplaceSubscriptionResponse?> GetByAzureSubscriptionIdAsync(string azureSubscriptionId);
    
    /// <summary>
    /// Submit customer information (Stage 4)
    /// </summary>
    Task<MarketplaceSubscriptionResponse> SubmitCustomerInfoAsync(SubmitCustomerInfoRequest request);
    
    /// <summary>
    /// Submit feature/token selections (Metered Billing stage)
    /// </summary>
    Task<MarketplaceSubscriptionResponse> SubmitFeatureSelectionAsync(SubmitFeatureSelectionRequest request);
    
    /// <summary>
    /// Finalize subscription and initiate async provisioning via IaC Runner.
    /// On success: saves deployment ID, sets status to Provisioning, sends preparation email.
    /// On failure: sets status to ProvisioningFailed, does NOT send email.
    /// </summary>
    Task<MarketplaceSubscriptionResponse> FinalizeSubscriptionAsync(int marketplaceSubscriptionId);
    
    /// <summary>
    /// Handle the webhook callback from IaC Runner when provisioning completes.
    /// On success: saves CCMS URL and metadata, sets status to Active, sends invitation email.
    /// On failure: sets status to ProvisioningFailed, does NOT send email.
    /// </summary>
    /// <param name="deploymentId">The deployment ID from the original provisioning request</param>
    /// <param name="success">Whether provisioning completed successfully</param>
    /// <param name="ccmsUrl">The CCMS access URL (only if success)</param>
    /// <param name="rawPayload">Raw JSON payload for storing dynamic metadata</param>
    /// <param name="errorMessage">Error message if provisioning failed</param>
    Task<MarketplaceSubscriptionResponse> HandleProvisioningCallbackAsync(
        string deploymentId, 
        bool success, 
        string? ccmsUrl, 
        string? rawPayload,
        string? errorMessage = null);
    
    /// <summary>
    /// Get all marketplace subscriptions (admin)
    /// </summary>
    Task<List<MarketplaceSubscriptionResponse>> GetAllAsync(int page = 1, int pageSize = 20);
}
