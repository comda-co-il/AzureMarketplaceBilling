namespace CcmsCommercialPlatform.Api.Models.Enums;

/// <summary>
/// Status of a marketplace subscription through the signup flow
/// </summary>
public enum MarketplaceSubscriptionStatus
{
    /// <summary>
    /// Token resolved, waiting for customer info
    /// </summary>
    PendingCustomerInfo = 0,
    
    /// <summary>
    /// Customer info collected, waiting for feature selection
    /// </summary>
    PendingFeatureSelection = 1,
    
    /// <summary>
    /// Feature selection complete, waiting for submission
    /// </summary>
    PendingSubmission = 2,
    
    /// <summary>
    /// Job saved to database, waiting for IaCRunner to pick it up via polling
    /// </summary>
    PendingProvisioning = 10,
    
    /// <summary>
    /// Provisioning request claimed by IaC Runner, waiting for webhook callback
    /// </summary>
    Provisioning = 3,
    
    /// <summary>
    /// Provisioning failed (either initial request or webhook reported failure)
    /// </summary>
    ProvisioningFailed = 4,
    
    /// <summary>
    /// Fully activated and ready to use (webhook confirmed success)
    /// </summary>
    Active = 5,
    
    /// <summary>
    /// Subscription was cancelled
    /// </summary>
    Cancelled = 6,
    
    /// <summary>
    /// Error occurred during processing
    /// </summary>
    Error = 7
}
