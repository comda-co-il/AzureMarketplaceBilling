using Microsoft.AspNetCore.Mvc;
using AzureMarketplaceBilling.Api.Data;
using AzureMarketplaceBilling.Api.Models.Enums;

namespace AzureMarketplaceBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<WebhookController> _logger;
    
    public WebhookController(AppDbContext context, ILogger<WebhookController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Simulate Azure Marketplace webhook for subscription changes
    /// </summary>
    [HttpPost("subscription-changed")]
    public async Task<IActionResult> SubscriptionChanged([FromBody] SubscriptionChangedWebhook webhook)
    {
        _logger.LogInformation(
            "Received subscription-changed webhook: SubscriptionId={SubscriptionId}, Action={Action}",
            webhook.SubscriptionId,
            webhook.Action);
        
        var subscription = await _context.Subscriptions.FindAsync(webhook.SubscriptionId);
        if (subscription == null)
        {
            return NotFound(new { message = $"Subscription '{webhook.SubscriptionId}' not found" });
        }
        
        switch (webhook.Action?.ToLower())
        {
            case "subscribe":
                subscription.Status = SubscriptionStatus.Active;
                break;
            case "unsubscribe":
                subscription.Status = SubscriptionStatus.Cancelled;
                subscription.EndDate = DateTime.UtcNow;
                break;
            case "suspend":
                subscription.Status = SubscriptionStatus.Suspended;
                break;
            case "reinstate":
                subscription.Status = SubscriptionStatus.Active;
                break;
            case "changeplan":
                if (!string.IsNullOrEmpty(webhook.PlanId))
                {
                    subscription.PlanId = webhook.PlanId;
                }
                break;
            default:
                return BadRequest(new { message = $"Unknown action: {webhook.Action}" });
        }
        
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Webhook processed successfully" });
    }
    
    /// <summary>
    /// Simulate Azure webhook for confirming usage was reported
    /// </summary>
    [HttpPost("usage-reported")]
    public async Task<IActionResult> UsageReported([FromBody] UsageReportedWebhook webhook)
    {
        _logger.LogInformation(
            "Received usage-reported webhook: UsageEventId={UsageEventId}, Status={Status}",
            webhook.UsageEventId,
            webhook.Status);
        
        var usageEvent = await _context.UsageEvents.FindAsync(webhook.UsageEventId);
        if (usageEvent == null)
        {
            return NotFound(new { message = $"Usage event '{webhook.UsageEventId}' not found" });
        }
        
        usageEvent.Status = webhook.Status ?? "Confirmed";
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Usage confirmation processed" });
    }
}

public class SubscriptionChangedWebhook
{
    public string SubscriptionId { get; set; } = string.Empty;
    public string? Action { get; set; }  // subscribe, unsubscribe, suspend, reinstate, changeplan
    public string? PlanId { get; set; }
    public string? OperationId { get; set; }
    public DateTime? ActivityTimestamp { get; set; }
}

public class UsageReportedWebhook
{
    public int UsageEventId { get; set; }
    public string? Status { get; set; }  // Accepted, Rejected
    public string? Message { get; set; }
}
