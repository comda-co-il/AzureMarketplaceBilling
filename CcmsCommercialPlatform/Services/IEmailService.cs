using CcmsCommercialPlatform.Api.Models;

namespace CcmsCommercialPlatform.Api.Services;

public interface IEmailService
{
    /// <summary>
    /// Send preparation email when provisioning starts (async flow)
    /// "Your CCMS environment is being prepared"
    /// </summary>
    Task<bool> SendPreparationEmailAsync(MarketplaceSubscription subscription);
    
    /// <summary>
    /// Send invitation email when provisioning completes successfully (from webhook callback)
    /// "Your CCMS is ready - here's the URL"
    /// </summary>
    Task<bool> SendInvitationEmailAsync(MarketplaceSubscription subscription);
    
    /// <summary>
    /// Send subscription confirmation email
    /// </summary>
    Task<bool> SendSubscriptionConfirmationAsync(MarketplaceSubscription subscription);
}
