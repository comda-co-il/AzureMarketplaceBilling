using Microsoft.EntityFrameworkCore;
using CcmsCommercialPlatform.Api.Data;
using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;
using CcmsCommercialPlatform.Api.Models.Enums;

namespace CcmsCommercialPlatform.Api.Services;

public class MarketplaceSubscriptionService : IMarketplaceSubscriptionService
{
    private readonly AppDbContext _context;
    private readonly IAzureMarketplaceClient _marketplaceClient;
    private readonly ILogger<MarketplaceSubscriptionService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEmailService _emailService;
    private readonly IIaCRunnerService _iacRunnerService;
    
    public MarketplaceSubscriptionService(
        AppDbContext context,
        IAzureMarketplaceClient marketplaceClient,
        ILogger<MarketplaceSubscriptionService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IEmailService emailService,
        IIaCRunnerService iacRunnerService)
    {
        _context = context;
        _marketplaceClient = marketplaceClient;
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _emailService = emailService;
        _iacRunnerService = iacRunnerService;
    }
    
    public async Task<ResolvedSubscriptionInfo> ResolveTokenAndCreateAsync(string token)
    {
        // First, resolve the token with Azure
        var resolved = await _marketplaceClient.ResolveTokenAsync(token);
        
        // Check if subscription already exists
        var existing = await _context.MarketplaceSubscriptions
            .FirstOrDefaultAsync(s => s.AzureSubscriptionId == resolved.SubscriptionId);
        
        if (existing != null)
        {
            _logger.LogInformation(
                "Marketplace subscription already exists for AzureSubscriptionId={AzureSubscriptionId}",
                resolved.SubscriptionId);
            
            resolved.MarketplaceSubscriptionId = existing.Id;
            return resolved;
        }
        
        // Create new marketplace subscription record
        var subscription = new MarketplaceSubscription
        {
            AzureSubscriptionId = resolved.SubscriptionId,
            MarketplaceToken = token,
            OfferId = resolved.OfferId,
            PlanId = resolved.PlanId,
            SubscriptionName = resolved.SubscriptionName,
            PurchaserEmail = resolved.Purchaser.EmailId,
            PurchaserTenantId = resolved.Purchaser.TenantId,
            PurchaserObjectId = resolved.Purchaser.ObjectId,
            BeneficiaryEmail = resolved.Beneficiary.EmailId,
            BeneficiaryTenantId = resolved.Beneficiary.TenantId,
            BeneficiaryObjectId = resolved.Beneficiary.ObjectId,
            Status = MarketplaceSubscriptionStatus.PendingCustomerInfo,
            TokenResolvedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.MarketplaceSubscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Created MarketplaceSubscription Id={Id} for AzureSubscriptionId={AzureSubscriptionId}",
            subscription.Id,
            resolved.SubscriptionId);
        
        resolved.MarketplaceSubscriptionId = subscription.Id;
        return resolved;
    }
    
    public async Task<MarketplaceSubscriptionResponse?> GetByIdAsync(int id)
    {
        var subscription = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        return subscription == null ? null : MapToResponse(subscription);
    }
    
    public async Task<MarketplaceSubscriptionResponse?> GetByAzureSubscriptionIdAsync(string azureSubscriptionId)
    {
        var subscription = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .FirstOrDefaultAsync(s => s.AzureSubscriptionId == azureSubscriptionId);
        
        return subscription == null ? null : MapToResponse(subscription);
    }
    
    public async Task<MarketplaceSubscriptionResponse> SubmitCustomerInfoAsync(SubmitCustomerInfoRequest request)
    {
        var subscription = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .FirstOrDefaultAsync(s => s.Id == request.MarketplaceSubscriptionId);
        
        if (subscription == null)
        {
            throw new ArgumentException($"Marketplace subscription '{request.MarketplaceSubscriptionId}' not found");
        }
        
        // Update customer info
        subscription.CustomerName = request.CustomerName;
        subscription.CustomerEmail = request.CustomerEmail;
        subscription.CompanyName = request.CompanyName;
        subscription.PhoneNumber = request.PhoneNumber;
        subscription.JobTitle = request.JobTitle;
        subscription.CountryCode = request.CountryCode;
        subscription.CountryOther = request.CountryOther;
        subscription.Comments = request.Comments;
        
        // Update Entra ID (Azure AD) configuration
        subscription.EntraClientId = request.EntraClientId;
        subscription.EntraClientSecret = request.EntraClientSecret;
        subscription.EntraTenantId = request.EntraTenantId;
        
        // Update IP whitelist
        subscription.WhitelistIps = string.Join(",", request.WhitelistIps ?? []);
        
        subscription.CustomerInfoSubmittedAt = DateTime.UtcNow;
        subscription.Status = MarketplaceSubscriptionStatus.PendingFeatureSelection;
        subscription.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Customer info submitted for MarketplaceSubscription Id={Id}",
            subscription.Id);
        
        return MapToResponse(subscription);
    }
    
    public async Task<MarketplaceSubscriptionResponse> SubmitFeatureSelectionAsync(SubmitFeatureSelectionRequest request)
    {
        var subscription = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .FirstOrDefaultAsync(s => s.Id == request.MarketplaceSubscriptionId);
        
        if (subscription == null)
        {
            throw new ArgumentException($"Marketplace subscription '{request.MarketplaceSubscriptionId}' not found");
        }
        
        // Remove existing feature selections
        _context.FeatureSelections.RemoveRange(subscription.FeatureSelections);
        
        // Add new feature selections
        foreach (var feature in request.Features)
        {
            var selection = new FeatureSelection
            {
                MarketplaceSubscriptionId = subscription.Id,
                FeatureId = feature.FeatureId,
                FeatureName = feature.FeatureName,
                IsEnabled = feature.IsEnabled,
                Quantity = feature.Quantity,
                PricePerUnit = feature.PricePerUnit,
                Notes = feature.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.FeatureSelections.Add(selection);
        }
        
        subscription.FeatureSelectionCompletedAt = DateTime.UtcNow;
        subscription.Status = MarketplaceSubscriptionStatus.PendingSubmission;
        subscription.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Feature selection submitted for MarketplaceSubscription Id={Id}, Features={FeatureCount}",
            subscription.Id,
            request.Features.Count);
        
        // Reload to get the new feature selections
        return (await GetByIdAsync(subscription.Id))!;
    }
    
    public async Task<MarketplaceSubscriptionResponse> FinalizeSubscriptionAsync(int marketplaceSubscriptionId)
    {
        var subscription = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .FirstOrDefaultAsync(s => s.Id == marketplaceSubscriptionId);
        
        if (subscription == null)
        {
            throw new ArgumentException($"Marketplace subscription '{marketplaceSubscriptionId}' not found");
        }
        
        // Call Claude IaC Runner API to provision infrastructure (async provisioning)
        _logger.LogInformation(
            "Calling IaC Runner to provision infrastructure for MarketplaceSubscription Id={Id}",
            subscription.Id);
        
        var iacRunnerResponse = await _iacRunnerService.ProvisionInfrastructureAsync(subscription);
        
        if (!iacRunnerResponse.Success)
        {
            // Provisioning request failed - do NOT send any email
            _logger.LogError(
                "IaC Runner provisioning request failed for MarketplaceSubscription Id={Id}. Message: {Message}",
                subscription.Id,
                iacRunnerResponse.Message);
            
            subscription.Status = MarketplaceSubscriptionStatus.ProvisioningFailed;
            subscription.ProvisioningErrorMessage = iacRunnerResponse.Message;
            subscription.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            throw new InvalidOperationException(
                $"Infrastructure provisioning request failed: {iacRunnerResponse.Message}");
        }
        
        // Provisioning request succeeded - save the deployment ID and update status
        _logger.LogInformation(
            "IaC Runner provisioning request accepted for MarketplaceSubscription Id={Id}. Deployment Id: {DeploymentId}",
            subscription.Id,
            iacRunnerResponse.Id);
        
        subscription.IaCDeploymentId = iacRunnerResponse.Id;
        subscription.Status = MarketplaceSubscriptionStatus.Provisioning;
        subscription.ProvisioningRequestedAt = DateTime.UtcNow;
        subscription.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        // Send preparation email: "Your CCMS environment is being prepared"
        try
        {
            var emailSent = await _emailService.SendPreparationEmailAsync(subscription);
            if (emailSent)
            {
                _logger.LogInformation(
                    "Preparation email sent to {CustomerEmail} for MarketplaceSubscription Id={Id}",
                    subscription.CustomerEmail,
                    subscription.Id);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to send preparation email to {CustomerEmail} for MarketplaceSubscription Id={Id}",
                    subscription.CustomerEmail,
                    subscription.Id);
            }
        }
        catch (Exception ex)
        {
            // Log but don't fail - the provisioning request was successful
            _logger.LogError(ex,
                "Error sending preparation email to {CustomerEmail} for MarketplaceSubscription Id={Id}",
                subscription.CustomerEmail,
                subscription.Id);
        }
        
        return MapToResponse(subscription);
    }
    
    public async Task<MarketplaceSubscriptionResponse> HandleProvisioningCallbackAsync(
        string deploymentId, 
        bool success, 
        string? ccmsUrl, 
        string? rawPayload,
        string? errorMessage = null)
    {
        // Find subscription by deployment ID
        var subscription = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .FirstOrDefaultAsync(s => s.IaCDeploymentId == deploymentId);
        
        if (subscription == null)
        {
            _logger.LogWarning(
                "Received provisioning callback for unknown deployment Id={DeploymentId}",
                deploymentId);
            throw new ArgumentException($"No subscription found for deployment '{deploymentId}'");
        }
        
        _logger.LogInformation(
            "Processing provisioning callback for MarketplaceSubscription Id={Id}, DeploymentId={DeploymentId}, Success={Success}",
            subscription.Id,
            deploymentId,
            success);
        
        // Store the raw payload for dynamic metadata (webhook structure is not guaranteed)
        subscription.ProvisioningMetadata = rawPayload;
        subscription.ProvisioningCompletedAt = DateTime.UtcNow;
        subscription.UpdatedAt = DateTime.UtcNow;
        
        if (success)
        {
            // Provisioning completed successfully
            subscription.CcmsUrl = ccmsUrl;
            subscription.Status = MarketplaceSubscriptionStatus.Active;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation(
                "Provisioning completed successfully for MarketplaceSubscription Id={Id}. CCMS URL: {CcmsUrl}",
                subscription.Id,
                ccmsUrl);
            
            // Notify Azure that the subscription is now active
            try
            {
                bool activated = await _marketplaceClient.ActivateSubscriptionAsync(
                    subscription.AzureSubscriptionId,
                    subscription.PlanId);
                
                if (activated)
                {
                    subscription.AzureActivatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation(
                        "Azure subscription activated for MarketplaceSubscription Id={Id}, AzureSubscriptionId={AzureSubscriptionId}",
                        subscription.Id,
                        subscription.AzureSubscriptionId);
                }
                else
                {
                    _logger.LogWarning(
                        "Failed to activate Azure subscription for MarketplaceSubscription Id={Id}, AzureSubscriptionId={AzureSubscriptionId}",
                        subscription.Id,
                        subscription.AzureSubscriptionId);
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail - the provisioning completed successfully
                _logger.LogError(ex,
                    "Error activating Azure subscription for MarketplaceSubscription Id={Id}, AzureSubscriptionId={AzureSubscriptionId}",
                    subscription.Id,
                    subscription.AzureSubscriptionId);
            }
            
            // Send invitation email: "Your CCMS is ready - here's the URL"
            try
            {
                bool emailSent = await _emailService.SendInvitationEmailAsync(subscription);
                if (emailSent)
                {
                    _logger.LogInformation(
                        "Invitation email sent to {CustomerEmail} for MarketplaceSubscription Id={Id}",
                        subscription.CustomerEmail,
                        subscription.Id);
                }
                else
                {
                    _logger.LogWarning(
                        "Failed to send invitation email to {CustomerEmail} for MarketplaceSubscription Id={Id}",
                        subscription.CustomerEmail,
                        subscription.Id);
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail - the provisioning completed
                _logger.LogError(ex,
                    "Error sending invitation email to {CustomerEmail} for MarketplaceSubscription Id={Id}",
                    subscription.CustomerEmail,
                    subscription.Id);
            }
        }
        else
        {
            // Provisioning failed - do NOT send invitation email
            subscription.Status = MarketplaceSubscriptionStatus.ProvisioningFailed;
            subscription.ProvisioningErrorMessage = errorMessage ?? "Provisioning failed (no error message provided)";
            
            await _context.SaveChangesAsync();
            
            _logger.LogError(
                "Provisioning failed for MarketplaceSubscription Id={Id}. Error: {Error}",
                subscription.Id,
                subscription.ProvisioningErrorMessage);
        }
        
        return MapToResponse(subscription);
    }
    
    public async Task<List<MarketplaceSubscriptionResponse>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var subscriptions = await _context.MarketplaceSubscriptions
            .Include(s => s.FeatureSelections)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return subscriptions.Select(MapToResponse).ToList();
    }
    
    private static MarketplaceSubscriptionResponse MapToResponse(MarketplaceSubscription subscription)
    {
        return new MarketplaceSubscriptionResponse
        {
            Id = subscription.Id,
            AzureSubscriptionId = subscription.AzureSubscriptionId,
            OfferId = subscription.OfferId,
            PlanId = subscription.PlanId,
            SubscriptionName = subscription.SubscriptionName,
            PurchaserEmail = subscription.PurchaserEmail,
            PurchaserTenantId = subscription.PurchaserTenantId,
            CustomerName = subscription.CustomerName,
            CustomerEmail = subscription.CustomerEmail,
            CompanyName = subscription.CompanyName,
            PhoneNumber = subscription.PhoneNumber,
            JobTitle = subscription.JobTitle,
            Country = subscription.CountryCode == "XX" && !string.IsNullOrEmpty(subscription.CountryOther) 
                ? subscription.CountryOther 
                : subscription.CountryCode, // For backward compatibility in response
            Comments = subscription.Comments,
            Status = subscription.Status,
            StatusDisplay = GetStatusDisplay(subscription.Status),
            FeatureSelections = subscription.FeatureSelections.Select(f => new FeatureSelectionResponse
            {
                Id = f.Id,
                FeatureId = f.FeatureId,
                FeatureName = f.FeatureName,
                IsEnabled = f.IsEnabled,
                Quantity = f.Quantity,
                PricePerUnit = f.PricePerUnit,
                Notes = f.Notes
            }).ToList(),
            CreatedAt = subscription.CreatedAt,
            CustomerInfoSubmittedAt = subscription.CustomerInfoSubmittedAt,
            FeatureSelectionCompletedAt = subscription.FeatureSelectionCompletedAt,
            SubmittedToExternalSystemAt = subscription.SubmittedToExternalSystemAt
        };
    }
    
    private static string GetStatusDisplay(MarketplaceSubscriptionStatus status)
    {
        return status switch
        {
            MarketplaceSubscriptionStatus.PendingCustomerInfo => "Pending Customer Info",
            MarketplaceSubscriptionStatus.PendingFeatureSelection => "Pending Feature Selection",
            MarketplaceSubscriptionStatus.PendingSubmission => "Pending Submission",
            MarketplaceSubscriptionStatus.Provisioning => "Provisioning",
            MarketplaceSubscriptionStatus.ProvisioningFailed => "Provisioning Failed",
            MarketplaceSubscriptionStatus.Active => "Active",
            MarketplaceSubscriptionStatus.Cancelled => "Cancelled",
            MarketplaceSubscriptionStatus.Error => "Error",
            _ => status.ToString()
        };
    }
}
