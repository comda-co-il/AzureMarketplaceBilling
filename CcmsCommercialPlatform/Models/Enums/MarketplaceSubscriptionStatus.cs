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
    /// Submitted to external system, waiting for activation
    /// </summary>
    SubmittedToExternalSystem = 3,
    
    /// <summary>
    /// Fully activated and ready to use
    /// </summary>
    Active = 4,
    
    /// <summary>
    /// Subscription was cancelled
    /// </summary>
    Cancelled = 5,
    
    /// <summary>
    /// Error occurred during processing
    /// </summary>
    Error = 6
}
