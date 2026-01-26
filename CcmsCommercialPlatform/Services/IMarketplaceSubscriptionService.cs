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
    /// Finalize subscription and submit to external system
    /// </summary>
    Task<MarketplaceSubscriptionResponse> FinalizeSubscriptionAsync(int marketplaceSubscriptionId);
    
    /// <summary>
    /// Get all marketplace subscriptions (admin)
    /// </summary>
    Task<List<MarketplaceSubscriptionResponse>> GetAllAsync(int page = 1, int pageSize = 20);
}
