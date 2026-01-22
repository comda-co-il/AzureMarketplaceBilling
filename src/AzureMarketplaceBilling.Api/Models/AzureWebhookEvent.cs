namespace AzureMarketplaceBilling.Api.Models;

/// <summary>
/// Stores raw webhook events received from Azure Marketplace for development/debugging.
/// This model captures all webhook data as-is before processing.
/// </summary>
public class AzureWebhookEvent
{
    public int Id { get; set; }
    
    /// <summary>
    /// The raw JSON payload received from Azure
    /// </summary>
    public string RawPayload { get; set; } = string.Empty;
    
    /// <summary>
    /// Azure operation action (e.g., Subscribe, Unsubscribe, Suspend, Reinstate, ChangePlan, ChangeQuantity, Renew)
    /// </summary>
    public string? Action { get; set; }
    
    /// <summary>
    /// Activity ID from Azure for tracing
    /// </summary>
    public string? ActivityId { get; set; }
    
    /// <summary>
    /// Azure Subscription ID
    /// </summary>
    public string? SubscriptionId { get; set; }
    
    /// <summary>
    /// Publisher ID from Azure Marketplace
    /// </summary>
    public string? PublisherId { get; set; }
    
    /// <summary>
    /// Offer ID from Azure Marketplace
    /// </summary>
    public string? OfferId { get; set; }
    
    /// <summary>
    /// Plan ID from Azure Marketplace
    /// </summary>
    public string? PlanId { get; set; }
    
    /// <summary>
    /// Operation ID for this specific operation
    /// </summary>
    public string? OperationId { get; set; }
    
    /// <summary>
    /// Status of the operation from Azure
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Timestamp when Azure sent this webhook
    /// </summary>
    public DateTime? AzureTimestamp { get; set; }
    
    /// <summary>
    /// When we received and stored this webhook
    /// </summary>
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// HTTP headers received with the webhook (for debugging)
    /// </summary>
    public string? Headers { get; set; }
    
    /// <summary>
    /// Source IP address of the webhook request
    /// </summary>
    public string? SourceIp { get; set; }
}
