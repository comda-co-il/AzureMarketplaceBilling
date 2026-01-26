using Microsoft.AspNetCore.Mvc;
using CcmsCommercialPlatform.Api.Models.DTOs;
using CcmsCommercialPlatform.Api.Services;

namespace CcmsCommercialPlatform.Api.Controllers;

/// <summary>
/// Controller for Azure Marketplace subscription signup flow
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MarketplaceController : ControllerBase
{
    private readonly IMarketplaceSubscriptionService _marketplaceService;
    private readonly ILogger<MarketplaceController> _logger;
    
    public MarketplaceController(
        IMarketplaceSubscriptionService marketplaceService,
        ILogger<MarketplaceController> logger)
    {
        _marketplaceService = marketplaceService;
        _logger = logger;
    }
    
    /// <summary>
    /// Stage 3: Resolve Azure Marketplace token and create subscription record
    /// </summary>
    [HttpPost("resolve")]
    public async Task<IActionResult> ResolveToken([FromBody] ResolveTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            _logger.LogInformation("Resolving marketplace token...");
            var result = await _marketplaceService.ResolveTokenAndCreateAsync(request.Token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve marketplace token");
            return BadRequest(new { message = "Failed to resolve token", error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get marketplace subscription by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubscription(int id)
    {
        var subscription = await _marketplaceService.GetByIdAsync(id);
        
        if (subscription == null)
        {
            return NotFound(new { message = $"Marketplace subscription '{id}' not found" });
        }
        
        return Ok(subscription);
    }
    
    /// <summary>
    /// Stage 4: Submit customer information
    /// </summary>
    [HttpPost("customer-info")]
    public async Task<IActionResult> SubmitCustomerInfo([FromBody] SubmitCustomerInfoRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var result = await _marketplaceService.SubmitCustomerInfoAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit customer info");
            return BadRequest(new { message = "Failed to submit customer info", error = ex.Message });
        }
    }
    
    /// <summary>
    /// Metered Billing Stage: Submit feature/token selections
    /// </summary>
    [HttpPost("features")]
    public async Task<IActionResult> SubmitFeatureSelection([FromBody] SubmitFeatureSelectionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var result = await _marketplaceService.SubmitFeatureSelectionAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit feature selection");
            return BadRequest(new { message = "Failed to submit feature selection", error = ex.Message });
        }
    }
    
    /// <summary>
    /// Finish: Finalize subscription and submit to external system
    /// </summary>
    [HttpPost("finalize")]
    public async Task<IActionResult> FinalizeSubscription([FromBody] FinalizeSubscriptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var result = await _marketplaceService.FinalizeSubscriptionAsync(request.MarketplaceSubscriptionId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to finalize subscription");
            return BadRequest(new { message = "Failed to finalize subscription", error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get all marketplace subscriptions (admin)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllSubscriptions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var subscriptions = await _marketplaceService.GetAllAsync(page, pageSize);
        return Ok(subscriptions);
    }
    
    /// <summary>
    /// Get available features for selection
    /// </summary>
    [HttpGet("available-features")]
    public IActionResult GetAvailableFeatures()
    {
        // Return the list of available features for metered billing
        // This could be loaded from configuration or database in the future
        var features = new[]
        {
            new
            {
                featureId = "desfire",
                featureName = "DESFire Tokens",
                description = "MIFARE DESFire EV2/EV3 credential tokens for high-security applications",
                pricePerUnit = 0.50m,
                minQuantity = 100,
                maxQuantity = 100000
            },
            new
            {
                featureId = "mifare",
                featureName = "MIFARE Classic Tokens",
                description = "MIFARE Classic credential tokens for standard access control",
                pricePerUnit = 0.30m,
                minQuantity = 100,
                maxQuantity = 100000
            },
            new
            {
                featureId = "seos",
                featureName = "SEOS Tokens",
                description = "HID SEOS credential tokens for mobile and physical credentials",
                pricePerUnit = 0.75m,
                minQuantity = 50,
                maxQuantity = 50000
            },
            new
            {
                featureId = "mobile",
                featureName = "Mobile Credentials",
                description = "Mobile credential tokens for smartphone-based access",
                pricePerUnit = 1.00m,
                minQuantity = 25,
                maxQuantity = 25000
            },
            new
            {
                featureId = "piv",
                featureName = "PIV Tokens",
                description = "PIV/CAC credential tokens for government and enterprise",
                pricePerUnit = 0.60m,
                minQuantity = 100,
                maxQuantity = 100000
            }
        };
        
        return Ok(features);
    }
}
