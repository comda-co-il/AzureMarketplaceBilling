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
    
    public MarketplaceSubscriptionService(
        AppDbContext context,
        IAzureMarketplaceClient marketplaceClient,
        ILogger<MarketplaceSubscriptionService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IEmailService emailService)
    {
        _context = context;
        _marketplaceClient = marketplaceClient;
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _emailService = emailService;
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
        
        // Get the external webhook URL from configuration
        var externalWebhookUrl = _configuration["ExternalSystem:WebhookUrl"];
        var ourWebhookUrl = _configuration["ExternalSystem:OurWebhookUrl"] 
            ?? $"{_configuration["ApplicationUrl"]}/api/webhook/external";
        
        if (!string.IsNullOrEmpty(externalWebhookUrl))
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                
                // Build the payload to send to external system
                var payload = new
                {
                    subscriptionId = subscription.Id,
                    azureSubscriptionId = subscription.AzureSubscriptionId,
                    offerId = subscription.OfferId,
                    planId = subscription.PlanId,
                    customer = new
                    {
                        name = subscription.CustomerName,
                        email = subscription.CustomerEmail,
                        company = subscription.CompanyName,
                        phone = subscription.PhoneNumber,
                        jobTitle = subscription.JobTitle,
                        countryCode = subscription.CountryCode,
                        countryOther = subscription.CountryOther,
                        comments = subscription.Comments
                    },
                    purchaser = new
                    {
                        email = subscription.PurchaserEmail,
                        tenantId = subscription.PurchaserTenantId
                    },
                    features = subscription.FeatureSelections.Select(f => new
                    {
                        featureId = f.FeatureId,
                        featureName = f.FeatureName,
                        isEnabled = f.IsEnabled,
                        quantity = f.Quantity,
                        pricePerUnit = f.PricePerUnit
                    }),
                    webhookUrl = ourWebhookUrl,
                    timestamp = DateTime.UtcNow
                };
                
                var response = await client.PostAsJsonAsync(externalWebhookUrl, payload);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Successfully submitted to external system. MarketplaceSubscription Id={Id}",
                        subscription.Id);
                }
                else
                {
                    _logger.LogWarning(
                        "External system returned {StatusCode}. MarketplaceSubscription Id={Id}",
                        response.StatusCode,
                        subscription.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to submit to external system. MarketplaceSubscription Id={Id}",
                    subscription.Id);
                // Continue anyway - we'll mark as submitted
            }
        }
        else
        {
            _logger.LogInformation(
                "No external webhook URL configured. Skipping external submission. MarketplaceSubscription Id={Id}",
                subscription.Id);
        }
        
        subscription.SubmittedToExternalSystemAt = DateTime.UtcNow;
        subscription.Status = MarketplaceSubscriptionStatus.SubmittedToExternalSystem;
        subscription.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        // Send welcome email to the customer
        try
        {
            var emailSent = await _emailService.SendWelcomeEmailAsync(subscription);
            if (emailSent)
            {
                _logger.LogInformation(
                    "Welcome email sent to {CustomerEmail} for MarketplaceSubscription Id={Id}",
                    subscription.CustomerEmail,
                    subscription.Id);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to send welcome email to {CustomerEmail} for MarketplaceSubscription Id={Id}",
                    subscription.CustomerEmail,
                    subscription.Id);
            }
        }
        catch (Exception ex)
        {
            // Log but don't fail the subscription finalization
            _logger.LogError(ex,
                "Error sending welcome email to {CustomerEmail} for MarketplaceSubscription Id={Id}",
                subscription.CustomerEmail,
                subscription.Id);
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
            MarketplaceSubscriptionStatus.SubmittedToExternalSystem => "Submitted",
            MarketplaceSubscriptionStatus.Active => "Active",
            MarketplaceSubscriptionStatus.Cancelled => "Cancelled",
            MarketplaceSubscriptionStatus.Error => "Error",
            _ => status.ToString()
        };
    }
}
