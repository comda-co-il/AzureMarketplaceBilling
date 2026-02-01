using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Identity;
using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;

namespace CcmsCommercialPlatform.Api.Services;

/// <summary>
/// Real Azure Marketplace client implementation that calls the SaaS Fulfillment API.
/// https://learn.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-subscription-api
/// </summary>
public class AzureMarketplaceClient : IAzureMarketplaceClient
{
    private readonly ILogger<AzureMarketplaceClient> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    
    // Azure Marketplace API base URL
    private const string MarketplaceApiBaseUrl = "https://marketplaceapi.microsoft.com/api";
    
    // Resource ID for Azure Marketplace API (used to acquire tokens)
    private const string MarketplaceResourceId = "20e940b3-4c77-4b0b-9a53-9e16a1b010a7";
    
    // Test token constant
    private const string TestToken = "test";
    
    /// <summary>
    /// Check if this is a test subscription (created from token="test")
    /// </summary>
    private static bool IsTestSubscription(string subscriptionId) =>
        subscriptionId.StartsWith("test-subscription-", StringComparison.OrdinalIgnoreCase) ||
        subscriptionId.StartsWith("demo-sub-", StringComparison.OrdinalIgnoreCase);
    
    public AzureMarketplaceClient(
        ILogger<AzureMarketplaceClient> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("AzureMarketplace");
    }
    
    /// <summary>
    /// Acquire an access token for the Azure Marketplace API
    /// </summary>
    private async Task<string> GetAccessTokenAsync()
    {
        var tenantId = _configuration["AzureMarketplace:TenantId"];
        var clientId = _configuration["AzureMarketplace:ClientId"];
        var clientSecret = _configuration["AzureMarketplace:ClientSecret"];
        
        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException(
                "Azure Marketplace credentials not configured. " +
                "Please set AzureMarketplace:TenantId, ClientId, and ClientSecret in appsettings.json");
        }
        
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        var tokenRequestContext = new Azure.Core.TokenRequestContext(
            new[] { $"{MarketplaceResourceId}/.default" });
        
        var token = await credential.GetTokenAsync(tokenRequestContext);
        return token.Token;
    }
    
    /// <summary>
    /// Get the API version from configuration
    /// </summary>
    private string GetApiVersion()
    {
        return _configuration["AzureMarketplace:ApiVersion"] ?? "2018-08-31";
    }
    
    /// <summary>
    /// Resolve an Azure Marketplace token to get subscription details
    /// https://learn.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-subscription-api#resolve-a-purchased-subscription
    /// </summary>
    public async Task<ResolvedSubscriptionInfo> ResolveTokenAsync(string token)
    {
        _logger.LogInformation("Resolving Azure Marketplace token...");
        
        // Special case: if token is "test", return mock data for testing
        // This works in both development and production for demo purposes
        if (token.Equals(TestToken, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("[TEST MODE] Returning mock subscription data for test token");
            await Task.CompletedTask; // Keep async signature
            return new ResolvedSubscriptionInfo
            {
                SubscriptionId = "test-subscription-" + Guid.NewGuid().ToString("N")[..8],
                SubscriptionName = "Test Subscription",
                OfferId = "comsigntrust-cms",
                PlanId = "professional",
                Purchaser = new PurchaserInfo
                {
                    EmailId = "test@example.com",
                    TenantId = Guid.NewGuid().ToString(),
                    ObjectId = Guid.NewGuid().ToString()
                },
                Beneficiary = new BeneficiaryInfo
                {
                    EmailId = "test@example.com",
                    TenantId = Guid.NewGuid().ToString(),
                    ObjectId = Guid.NewGuid().ToString()
                }
            };
        }
        
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var apiVersion = GetApiVersion();
            
            var request = new HttpRequestMessage(
                HttpMethod.Post, 
                $"{MarketplaceApiBaseUrl}/saas/subscriptions/resolve?api-version={apiVersion}");
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            // URL decode the token - Azure Marketplace tokens come URL-encoded from the query string
            var decodedToken = WebUtility.UrlDecode(token);
            request.Headers.Add("x-ms-marketplace-token", decodedToken);
            request.Headers.Add("x-ms-requestid", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-correlationid", Guid.NewGuid().ToString());
            request.Content = new StringContent("", Encoding.UTF8, "application/json");
            
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Failed to resolve marketplace token. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, responseContent);
                throw new HttpRequestException(
                    $"Azure Marketplace API returned {response.StatusCode}: {responseContent}");
            }
            
            _logger.LogInformation("Successfully resolved marketplace token");
            
            // Parse the response
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resolveResponse = JsonSerializer.Deserialize<MarketplaceResolveResponse>(responseContent, options);
            
            if (resolveResponse == null)
            {
                throw new InvalidOperationException("Failed to parse marketplace resolve response");
            }
            
            // Map to our DTO
            var result = new ResolvedSubscriptionInfo
            {
                SubscriptionId = resolveResponse.Id ?? resolveResponse.SubscriptionId ?? "",
                SubscriptionName = resolveResponse.SubscriptionName ?? resolveResponse.Name ?? "",
                OfferId = resolveResponse.OfferId ?? "",
                PlanId = resolveResponse.PlanId ?? "",
                Purchaser = new PurchaserInfo
                {
                    EmailId = resolveResponse.Purchaser?.EmailId ?? "",
                    TenantId = resolveResponse.Purchaser?.TenantId ?? "",
                    ObjectId = resolveResponse.Purchaser?.ObjectId ?? ""
                },
                Beneficiary = new BeneficiaryInfo
                {
                    EmailId = resolveResponse.Beneficiary?.EmailId ?? "",
                    TenantId = resolveResponse.Beneficiary?.TenantId ?? "",
                    ObjectId = resolveResponse.Beneficiary?.ObjectId ?? ""
                }
            };
            
            _logger.LogInformation(
                "Resolved subscription: Id={SubscriptionId}, Name={Name}, Plan={PlanId}",
                result.SubscriptionId, result.SubscriptionName, result.PlanId);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving marketplace token");
            throw;
        }
    }
    
    /// <summary>
    /// Activate a subscription after the landing page flow is complete
    /// https://learn.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-subscription-api#activate-a-subscription
    /// </summary>
    public async Task<bool> ActivateSubscriptionAsync(string subscriptionId, string planId)
    {
        _logger.LogInformation(
            "Activating subscription: {SubscriptionId} with plan: {PlanId}",
            subscriptionId, planId);
        
        // Handle test subscriptions - skip real API call
        if (IsTestSubscription(subscriptionId))
        {
            _logger.LogWarning("[TEST MODE] Skipping Azure activation for test subscription {SubscriptionId}", subscriptionId);
            await Task.CompletedTask;
            return true;
        }
        
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var apiVersion = GetApiVersion();
            
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{MarketplaceApiBaseUrl}/saas/subscriptions/{subscriptionId}/activate?api-version={apiVersion}");
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("x-ms-requestid", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-correlationid", Guid.NewGuid().ToString());
            
            var body = new { planId = planId };
            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Failed to activate subscription. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, responseContent);
                return false;
            }
            
            _logger.LogInformation("Successfully activated subscription {SubscriptionId}", subscriptionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating subscription {SubscriptionId}", subscriptionId);
            throw;
        }
    }
    
    /// <summary>
    /// Report usage to the Azure Marketplace Metered Billing API
    /// https://learn.microsoft.com/en-us/azure/marketplace/marketplace-metering-service-apis
    /// </summary>
    public async Task<bool> ReportUsageAsync(UsageEvent usageEvent)
    {
        _logger.LogInformation(
            "Reporting usage: ResourceId={ResourceId}, Dimension={Dimension}, Quantity={Quantity}",
            usageEvent.ResourceId, usageEvent.Dimension, usageEvent.Quantity);
        
        // Handle test subscriptions - skip real API call
        if (IsTestSubscription(usageEvent.ResourceId))
        {
            _logger.LogWarning("[TEST MODE] Skipping Azure usage reporting for test subscription {ResourceId}", usageEvent.ResourceId);
            await Task.CompletedTask;
            return true;
        }
        
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var apiVersion = GetApiVersion();
            
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{MarketplaceApiBaseUrl}/usageEvent?api-version={apiVersion}");
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("x-ms-requestid", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-correlationid", Guid.NewGuid().ToString());
            
            var body = new
            {
                resourceId = usageEvent.ResourceId,
                planId = usageEvent.PlanId,
                dimension = usageEvent.Dimension,
                quantity = usageEvent.Quantity,
                effectiveStartTime = usageEvent.EffectiveStartTime.ToString("o")
            };
            
            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Failed to report usage. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, responseContent);
                return false;
            }
            
            _logger.LogInformation(
                "Successfully reported usage for {ResourceId}, dimension {Dimension}",
                usageEvent.ResourceId, usageEvent.Dimension);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting usage");
            throw;
        }
    }
    
    /// <summary>
    /// Update a subscription's plan
    /// https://learn.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-subscription-api#change-the-plan-on-the-subscription
    /// </summary>
    public async Task<bool> UpdateSubscriptionAsync(string subscriptionId, string planId)
    {
        _logger.LogInformation(
            "Updating subscription {SubscriptionId} to plan {PlanId}",
            subscriptionId, planId);
        
        // Handle test subscriptions - skip real API call
        if (IsTestSubscription(subscriptionId))
        {
            _logger.LogWarning("[TEST MODE] Skipping Azure update for test subscription {SubscriptionId}", subscriptionId);
            await Task.CompletedTask;
            return true;
        }
        
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var apiVersion = GetApiVersion();
            
            var request = new HttpRequestMessage(
                HttpMethod.Patch,
                $"{MarketplaceApiBaseUrl}/saas/subscriptions/{subscriptionId}?api-version={apiVersion}");
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("x-ms-requestid", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-correlationid", Guid.NewGuid().ToString());
            
            var body = new { planId = planId };
            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");
            
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Failed to update subscription. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, responseContent);
                return false;
            }
            
            _logger.LogInformation("Successfully updated subscription {SubscriptionId}", subscriptionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subscription {SubscriptionId}", subscriptionId);
            throw;
        }
    }
    
    /// <summary>
    /// Cancel a subscription
    /// https://learn.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-subscription-api#cancel-a-subscription
    /// </summary>
    public async Task<bool> CancelSubscriptionAsync(string subscriptionId)
    {
        _logger.LogInformation("Cancelling subscription {SubscriptionId}", subscriptionId);
        
        // Handle test subscriptions - skip real API call
        if (IsTestSubscription(subscriptionId))
        {
            _logger.LogWarning("[TEST MODE] Skipping Azure cancellation for test subscription {SubscriptionId}", subscriptionId);
            await Task.CompletedTask;
            return true;
        }
        
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var apiVersion = GetApiVersion();
            
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"{MarketplaceApiBaseUrl}/saas/subscriptions/{subscriptionId}?api-version={apiVersion}");
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add("x-ms-requestid", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-correlationid", Guid.NewGuid().ToString());
            
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Failed to cancel subscription. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, responseContent);
                return false;
            }
            
            _logger.LogInformation("Successfully cancelled subscription {SubscriptionId}", subscriptionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling subscription {SubscriptionId}", subscriptionId);
            throw;
        }
    }
}

/// <summary>
/// Response model for the Azure Marketplace Resolve API
/// </summary>
internal class MarketplaceResolveResponse
{
    public string? Id { get; set; }
    public string? SubscriptionId { get; set; }
    public string? SubscriptionName { get; set; }
    public string? Name { get; set; }
    public string? OfferId { get; set; }
    public string? PlanId { get; set; }
    public string? PublisherId { get; set; }
    public string? SaasSubscriptionStatus { get; set; }
    public MarketplaceUserInfo? Purchaser { get; set; }
    public MarketplaceUserInfo? Beneficiary { get; set; }
}

internal class MarketplaceUserInfo
{
    public string? EmailId { get; set; }
    public string? TenantId { get; set; }
    public string? ObjectId { get; set; }
}
