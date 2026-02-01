using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;
using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Services;

/// <summary>
/// IaC Runner service implementation using polling architecture.
/// Instead of POSTing to a private network, this service queues jobs in the database.
/// The IaCRunner polls the Azure app for pending jobs via GET /api/iac/pending-jobs.
/// </summary>
public class IaCRunnerService : IIaCRunnerService
{
    private readonly ILogger<IaCRunnerService> _logger;

    public IaCRunnerService(ILogger<IaCRunnerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Queue the subscription for provisioning by IaCRunner.
    /// The IaCRunner will poll for pending jobs and claim this one.
    /// </summary>
    public Task<IaCRunnerResponse> ProvisionInfrastructureAsync(MarketplaceSubscription subscription)
    {
        // Generate a temporary deployment ID (will be replaced when IaCRunner claims the job)
        string deploymentId = $"pending-{subscription.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}";

        _logger.LogInformation(
            "Queuing provisioning job for subscription {SubscriptionId}, Company: {CompanyName}. " +
            "Job will be picked up by IaCRunner via polling.",
            subscription.Id,
            subscription.CompanyName);

        // Return success - the job is now queued for IaCRunner to pick up
        // Note: The actual status change to PendingProvisioning happens in the caller (MarketplaceSubscriptionService)
        // We return a response indicating the job is queued, not completed
        return Task.FromResult(new IaCRunnerResponse
        {
            Id = deploymentId,
            Success = true,
            Message = "Job queued for IaCRunner. Waiting for runner to claim and process."
        });
    }
}
