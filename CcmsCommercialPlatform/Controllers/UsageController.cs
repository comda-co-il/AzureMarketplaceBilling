using Microsoft.AspNetCore.Mvc;
using CcmsCommercialPlatform.Api.Models.DTOs;
using CcmsCommercialPlatform.Api.Models.Enums;
using CcmsCommercialPlatform.Api.Services;

namespace CcmsCommercialPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsageController : ControllerBase
{
    private readonly IMeteredBillingService _meteredBillingService;
    
    public UsageController(IMeteredBillingService meteredBillingService)
    {
        _meteredBillingService = meteredBillingService;
    }
    
    /// <summary>
    /// Record a usage event (simulates operation)
    /// </summary>
    [HttpPost("record")]
    public async Task<IActionResult> RecordUsage([FromBody] RecordUsageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var usageRecord = await _meteredBillingService.RecordUsageAsync(
                request.SubscriptionId,
                request.DimensionType,
                request.Quantity);
            
            return Ok(new
            {
                message = "Usage recorded successfully",
                usageRecord = new
                {
                    usageRecord.DimensionType,
                    usageRecord.DimensionId,
                    usageRecord.UsedQuantity,
                    usageRecord.ReportedOverage,
                    usageRecord.LastUpdated
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get usage summary for current billing period
    /// </summary>
    [HttpGet("{subscriptionId}")]
    public async Task<IActionResult> GetUsageSummary(string subscriptionId)
    {
        try
        {
            var usage = await _meteredBillingService.GetUsageSummaryAsync(subscriptionId);
            return Ok(usage);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get historical usage records
    /// </summary>
    [HttpGet("{subscriptionId}/history")]
    public async Task<IActionResult> GetUsageHistory(
        string subscriptionId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] TokenUsageType? dimensionType = null)
    {
        var history = await _meteredBillingService.GetUsageHistoryAsync(
            subscriptionId,
            startDate,
            endDate,
            dimensionType);
        
        return Ok(history);
    }
}
