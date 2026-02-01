using System.Net.Http.Json;
using System.Text.Json;
using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.DTOs;

namespace CcmsCommercialPlatform.Api.Services;

/// <summary>
/// Claude IaC Runner API client implementation
/// Automated CCMS infrastructure provisioning powered by Claude Code CLI
/// </summary>
public class IaCRunnerService : IIaCRunnerService
{
    private readonly ILogger<IaCRunnerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    
    public IaCRunnerService(
        ILogger<IaCRunnerService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IaCRunnerResponse> ProvisionInfrastructureAsync(MarketplaceSubscription subscription)
    {
        var apiUrl = _configuration["IaCRunner:ApiUrl"];
        
        if (string.IsNullOrEmpty(apiUrl))
        {
            _logger.LogError("IaC Runner API URL is not configured. Please set IaCRunner:ApiUrl in appsettings.json");
            return new IaCRunnerResponse
            {
                Id = string.Empty,
                Success = false,
                Message = "IaC Runner API URL is not configured"
            };
        }
        
        // Build the request payload with all collected form data
        var request = new IaCRunnerRequest
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
            WebhookUrl = $"{_configuration["ApplicationUrl"]?.TrimEnd('/')}/api/webhook/ccms-provisioning",
            Timestamp = DateTime.UtcNow
        };
        
        try
        {
            _logger.LogInformation(
                "Calling IaC Runner API to provision infrastructure for subscription {SubscriptionId}, Company: {CompanyName}",
                subscription.Id,
                subscription.CompanyName);
            
            var client = _httpClientFactory.CreateClient("IaCRunner");
            
            // Set timeout from configuration or default to 60 seconds
            var timeoutSeconds = _configuration.GetValue<int>("IaCRunner:TimeoutSeconds", 60);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            
            var response = await client.PostAsJsonAsync(apiUrl, request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogDebug("IaC Runner API response status: {StatusCode}, content: {Content}",
                response.StatusCode,
                responseContent);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "IaC Runner API returned error. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode,
                    responseContent);
                
                // Try to parse error response
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<IaCRunnerResponse>(responseContent, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (errorResponse != null)
                    {
                        return errorResponse;
                    }
                }
                catch
                {
                    // If parsing fails, return generic error
                }
                
                return new IaCRunnerResponse
                {
                    Id = string.Empty,
                    Success = false,
                    Message = $"IaC Runner API returned error: {response.StatusCode} - {responseContent}"
                };
            }
            
            // Parse successful response
            var result = JsonSerializer.Deserialize<IaCRunnerResponse>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (result == null)
            {
                _logger.LogError("Failed to parse IaC Runner API response: {Content}", responseContent);
                return new IaCRunnerResponse
                {
                    Id = string.Empty,
                    Success = false,
                    Message = "Failed to parse IaC Runner API response"
                };
            }
            
            _logger.LogInformation(
                "IaC Runner API response - Success: {Success}, Id: {Id}, Message: {Message}",
                result.Success,
                result.Id,
                result.Message);
            
            return result;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "IaC Runner API request timed out for subscription {SubscriptionId}", subscription.Id);
            return new IaCRunnerResponse
            {
                Id = string.Empty,
                Success = false,
                Message = "IaC Runner API request timed out"
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling IaC Runner API for subscription {SubscriptionId}", subscription.Id);
            return new IaCRunnerResponse
            {
                Id = string.Empty,
                Success = false,
                Message = $"Network error calling IaC Runner API: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling IaC Runner API for subscription {SubscriptionId}", subscription.Id);
            return new IaCRunnerResponse
            {
                Id = string.Empty,
                Success = false,
                Message = $"Unexpected error: {ex.Message}"
            };
        }
    }
}
