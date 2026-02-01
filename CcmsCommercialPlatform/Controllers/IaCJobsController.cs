using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CcmsCommercialPlatform.Api.Data;
using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;
using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Controllers;

/// <summary>
/// API endpoints for IaCRunner to poll for pending provisioning jobs.
/// The IaCRunner polls this endpoint from the private network (outbound to public Azure app).
/// </summary>
[ApiController]
[Route("api/iac")]
public class IaCJobsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<IaCJobsController> _logger;
    private readonly IConfiguration _configuration;

    public IaCJobsController(
        AppDbContext context,
        ILogger<IaCJobsController> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Validates the API key from the request header.
    /// </summary>
    private bool ValidateApiKey()
    {
        string? configuredApiKey = _configuration["IaCRunner:ApiKey"];
        
        // If no API key is configured, allow all requests (development mode)
        if (string.IsNullOrEmpty(configuredApiKey))
        {
            _logger.LogWarning("IaCRunner:ApiKey is not configured. Allowing unauthenticated access.");
            return true;
        }

        // Check for API key in header
        if (!Request.Headers.TryGetValue("X-Api-Key", out Microsoft.Extensions.Primitives.StringValues apiKeyHeader))
        {
            return false;
        }

        return apiKeyHeader.ToString() == configuredApiKey;
    }

    /// <summary>
    /// Get all pending provisioning jobs waiting to be picked up by IaCRunner.
    /// IaCRunner should poll this endpoint periodically (e.g., every 15 seconds).
    /// </summary>
    [HttpGet("pending-jobs")]
    public async Task<IActionResult> GetPendingJobs()
    {
        if (!ValidateApiKey())
        {
            _logger.LogWarning("Unauthorized access attempt to pending-jobs endpoint");
            return Unauthorized(new { message = "Invalid or missing API key" });
        }

        List<MarketplaceSubscription> pendingSubscriptions = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .Where(s => s.Status == MarketplaceSubscriptionStatus.PendingProvisioning)
            .OrderBy(s => s.ProvisioningRequestedAt ?? s.CreatedAt)
            .ToListAsync();

        List<IaCPendingJobDto> jobs = pendingSubscriptions.Select(s => new IaCPendingJobDto
        {
            SubscriptionId = s.Id,
            AzureSubscriptionId = s.AzureSubscriptionId,
            CompanyName = s.CompanyName,
            CustomerEmail = s.CustomerEmail,
            CreatedAt = s.ProvisioningRequestedAt ?? s.CreatedAt
        }).ToList();

        _logger.LogDebug("Returning {Count} pending provisioning jobs", jobs.Count);

        return Ok(new { jobs, count = jobs.Count });
    }

    /// <summary>
    /// Claim a pending job for processing. This prevents other runners from picking up the same job.
    /// Returns the full job details needed for provisioning.
    /// </summary>
    [HttpPost("claim-job/{subscriptionId}")]
    public async Task<IActionResult> ClaimJob(int subscriptionId)
    {
        if (!ValidateApiKey())
        {
            _logger.LogWarning("Unauthorized access attempt to claim-job endpoint");
            return Unauthorized(new { message = "Invalid or missing API key" });
        }

        MarketplaceSubscription? subscription = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .FirstOrDefaultAsync(s => s.Id == subscriptionId);

        if (subscription == null)
        {
            return NotFound(new { message = $"Subscription {subscriptionId} not found" });
        }

        // Check if the job is still pending (not already claimed by another runner)
        if (subscription.Status != MarketplaceSubscriptionStatus.PendingProvisioning)
        {
            _logger.LogWarning(
                "Job {SubscriptionId} cannot be claimed - current status is {Status}",
                subscriptionId,
                subscription.Status);

            return Conflict(new 
            { 
                message = $"Job {subscriptionId} is not available for claiming",
                currentStatus = subscription.Status.ToString()
            });
        }

        // Claim the job by updating status to Provisioning
        subscription.Status = MarketplaceSubscriptionStatus.Provisioning;
        subscription.UpdatedAt = DateTime.UtcNow;

        // Generate a deployment ID for tracking
        string deploymentId = $"deploy-{subscriptionId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        subscription.IaCDeploymentId = deploymentId;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Job {SubscriptionId} claimed successfully. DeploymentId: {DeploymentId}, Company: {CompanyName}",
            subscriptionId,
            deploymentId,
            subscription.CompanyName);

        // Build the full job payload (same structure as the old IaCRunnerRequest)
        IaCRunnerRequest jobPayload = BuildJobPayload(subscription, deploymentId);

        return Ok(new 
        { 
            message = "Job claimed successfully",
            deploymentId,
            job = jobPayload
        });
    }

    /// <summary>
    /// Get full details of a specific job (for retries or status checks).
    /// </summary>
    [HttpGet("job/{subscriptionId}")]
    public async Task<IActionResult> GetJob(int subscriptionId)
    {
        if (!ValidateApiKey())
        {
            _logger.LogWarning("Unauthorized access attempt to get-job endpoint");
            return Unauthorized(new { message = "Invalid or missing API key" });
        }

        MarketplaceSubscription? subscription = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .FirstOrDefaultAsync(s => s.Id == subscriptionId);

        if (subscription == null)
        {
            return NotFound(new { message = $"Subscription {subscriptionId} not found" });
        }

        IaCRunnerRequest jobPayload = BuildJobPayload(subscription, subscription.IaCDeploymentId ?? $"deploy-{subscriptionId}");

        return Ok(new
        {
            subscriptionId = subscription.Id,
            status = subscription.Status.ToString(),
            deploymentId = subscription.IaCDeploymentId,
            job = jobPayload
        });
    }

    /// <summary>
    /// Build the job payload from a subscription (same format as IaCRunnerRequest).
    /// </summary>
    private IaCRunnerRequest BuildJobPayload(MarketplaceSubscription subscription, string deploymentId)
    {
        string webhookUrl = $"{_configuration["ApplicationUrl"]?.TrimEnd('/')}/api/webhook/ccms-provisioning";

        return new IaCRunnerRequest
        {
            SubscriptionId = subscription.Id,
            AzureSubscriptionId = subscription.AzureSubscriptionId,
            OfferId = subscription.OfferId,
            PlanId = subscription.PlanId,
            Customer = new IaCRunnerCustomerInfo
            {
                Name = subscription.CustomerName,
                Email = subscription.CustomerEmail,
                Company = subscription.CompanyName,
                Phone = subscription.PhoneNumber,
                JobTitle = subscription.JobTitle,
                CountryCode = subscription.CountryCode,
                CountryOther = subscription.CountryOther,
                Comments = subscription.Comments
            },
            EntraConfig = new IaCRunnerEntraConfig
            {
                ClientId = subscription.EntraClientId,
                ClientSecret = subscription.EntraClientSecret,
                TenantId = subscription.EntraTenantId,
                AdminGroupObjectId = subscription.EntraAdminGroupObjectId
            },
            Purchaser = new IaCRunnerPurchaserInfo
            {
                Email = subscription.PurchaserEmail,
                TenantId = subscription.PurchaserTenantId,
                ObjectId = subscription.PurchaserObjectId
            },
            Features = subscription.FeatureSelections.Select(f => new IaCRunnerFeature
            {
                FeatureId = f.FeatureId,
                FeatureName = f.FeatureName,
                IsEnabled = f.IsEnabled,
                Quantity = f.Quantity,
                PricePerUnit = f.PricePerUnit
            }).ToList(),
            WhitelistIps = string.IsNullOrEmpty(subscription.WhitelistIps)
                ? []
                : [.. subscription.WhitelistIps.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)],
            WebhookUrl = webhookUrl,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Lightweight DTO for listing pending jobs (without full details).
/// </summary>
public class IaCPendingJobDto
{
    public int SubscriptionId { get; set; }
    public string AzureSubscriptionId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
