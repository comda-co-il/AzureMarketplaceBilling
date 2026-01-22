using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using CcmsCommercialPlatform.Api.Data;
using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Controllers;

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
    /// Azure Marketplace Connection Webhook endpoint.
    /// Receives POST requests from Azure and stores them for development/debugging.
    /// This is the endpoint to configure in Azure Partner Center -> Technical Configuration -> Connection Webhook.
    /// </summary>
    [HttpPost("azure")]
    public async Task<IActionResult> AzureWebhook()
    {
        try
        {
            // Read the raw request body
            using var reader = new StreamReader(Request.Body);
            var rawPayload = await reader.ReadToEndAsync();
            
            _logger.LogInformation("Received Azure webhook: {Payload}", rawPayload);
            
            // Extract headers for debugging
            var headers = string.Join("; ", Request.Headers.Select(h => $"{h.Key}: {h.Value}"));
            
            // Get source IP
            var sourceIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            // Try to parse the payload to extract common fields
            string? action = null;
            string? activityId = null;
            string? subscriptionId = null;
            string? publisherId = null;
            string? offerId = null;
            string? planId = null;
            string? operationId = null;
            string? status = null;
            DateTime? azureTimestamp = null;
            
            try
            {
                using var doc = JsonDocument.Parse(rawPayload);
                var root = doc.RootElement;
                
                action = GetJsonString(root, "action") ?? GetJsonString(root, "Action");
                activityId = GetJsonString(root, "activityId") ?? GetJsonString(root, "ActivityId");
                subscriptionId = GetJsonString(root, "subscriptionId") ?? GetJsonString(root, "SubscriptionId") ?? GetJsonString(root, "id");
                publisherId = GetJsonString(root, "publisherId") ?? GetJsonString(root, "PublisherId");
                offerId = GetJsonString(root, "offerId") ?? GetJsonString(root, "OfferId");
                planId = GetJsonString(root, "planId") ?? GetJsonString(root, "PlanId");
                operationId = GetJsonString(root, "operationId") ?? GetJsonString(root, "OperationId");
                status = GetJsonString(root, "status") ?? GetJsonString(root, "Status");
                
                var timestampStr = GetJsonString(root, "timeStamp") ?? GetJsonString(root, "timestamp") ?? GetJsonString(root, "Timestamp");
                if (!string.IsNullOrEmpty(timestampStr) && DateTime.TryParse(timestampStr, out var ts))
                {
                    azureTimestamp = ts;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse webhook JSON payload");
            }
            
            // Store the webhook event
            var webhookEvent = new AzureWebhookEvent
            {
                RawPayload = rawPayload,
                Action = action,
                ActivityId = activityId,
                SubscriptionId = subscriptionId,
                PublisherId = publisherId,
                OfferId = offerId,
                PlanId = planId,
                OperationId = operationId,
                Status = status,
                AzureTimestamp = azureTimestamp,
                ReceivedAt = DateTime.UtcNow,
                Headers = headers,
                SourceIp = sourceIp
            };
            
            _context.AzureWebhookEvents.Add(webhookEvent);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation(
                "Stored Azure webhook event: Id={Id}, Action={Action}, SubscriptionId={SubscriptionId}",
                webhookEvent.Id, action, subscriptionId);
            
            // Return success - Azure expects 200 OK
            return Ok(new { message = "Webhook received and stored", eventId = webhookEvent.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Azure webhook");
            return StatusCode(500, new { message = "Error processing webhook", error = ex.Message });
        }
    }
    
    private static string? GetJsonString(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String)
        {
            return prop.GetString();
        }
        return null;
    }
    
    /// <summary>
    /// Get all stored Azure webhook events for development/debugging.
    /// </summary>
    [HttpGet("azure/events")]
    public async Task<IActionResult> GetAzureWebhookEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.AzureWebhookEvents
            .OrderByDescending(e => e.ReceivedAt);
        
        var total = await query.CountAsync();
        var events = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new
        {
            data = events,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling((double)total / pageSize)
        });
    }
    
    /// <summary>
    /// Get a specific Azure webhook event by ID.
    /// </summary>
    [HttpGet("azure/events/{id}")]
    public async Task<IActionResult> GetAzureWebhookEvent(int id)
    {
        var webhookEvent = await _context.AzureWebhookEvents.FindAsync(id);
        if (webhookEvent == null)
        {
            return NotFound(new { message = $"Webhook event '{id}' not found" });
        }
        return Ok(webhookEvent);
    }
    
    /// <summary>
    /// Delete all stored Azure webhook events (for cleanup during development).
    /// </summary>
    [HttpDelete("azure/events")]
    public async Task<IActionResult> ClearAzureWebhookEvents()
    {
        var count = await _context.AzureWebhookEvents.CountAsync();
        _context.AzureWebhookEvents.RemoveRange(_context.AzureWebhookEvents);
        await _context.SaveChangesAsync();
        
        return Ok(new { message = $"Deleted {count} webhook events" });
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
