using CcmsCommercialPlatform.Api.Models;

namespace CcmsCommercialPlatform.Api.Services;

public interface IEmailService
{
    /// <summary>
    /// Send a welcome email to a new subscriber
    /// </summary>
    Task<bool> SendWelcomeEmailAsync(MarketplaceSubscription subscription);
    
    /// <summary>
    /// Send subscription confirmation email
    /// </summary>
    Task<bool> SendSubscriptionConfirmationAsync(MarketplaceSubscription subscription);
}
