using Microsoft.AspNetCore.Mvc;
using AzureMarketplaceBilling.Api.Models.DTOs;
using AzureMarketplaceBilling.Api.Services;

namespace AzureMarketplaceBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IMeteredBillingService _meteredBillingService;
    
    public SubscriptionsController(
        ISubscriptionService subscriptionService,
        IMeteredBillingService meteredBillingService)
    {
        _subscriptionService = subscriptionService;
        _meteredBillingService = meteredBillingService;
    }
    
    /// <summary>
    /// Create a new subscription (Landing Page signup)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var subscription = await _subscriptionService.CreateSubscriptionAsync(request);
            return CreatedAtAction(nameof(GetSubscription), new { id = subscription.Id }, subscription);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get subscription details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubscription(string id)
    {
        var subscription = await _subscriptionService.GetSubscriptionAsync(id);
        
        if (subscription == null)
        {
            return NotFound(new { message = $"Subscription '{id}' not found" });
        }
        
        return Ok(subscription);
    }
    
    /// <summary>
    /// Get current usage for subscription
    /// </summary>
    [HttpGet("{id}/usage")]
    public async Task<IActionResult> GetSubscriptionUsage(string id)
    {
        try
        {
            var usage = await _meteredBillingService.GetUsageSummaryAsync(id);
            return Ok(usage);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Upgrade or downgrade subscription plan
    /// </summary>
    [HttpPut("{id}/plan")]
    public async Task<IActionResult> ChangePlan(string id, [FromBody] ChangePlanRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var subscription = await _subscriptionService.ChangePlanAsync(id, request.NewPlanId);
            
            if (subscription == null)
            {
                return NotFound(new { message = $"Subscription '{id}' not found" });
            }
            
            return Ok(subscription);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Cancel subscription
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelSubscription(string id)
    {
        var result = await _subscriptionService.CancelSubscriptionAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Subscription '{id}' not found" });
        }
        
        return Ok(new { message = "Subscription cancelled successfully" });
    }
    
    /// <summary>
    /// Start a new billing period (reset quotas)
    /// </summary>
    [HttpPost("{id}/new-billing-period")]
    public async Task<IActionResult> StartNewBillingPeriod(string id)
    {
        var subscription = await _subscriptionService.StartNewBillingPeriodAsync(id);
        
        if (subscription == null)
        {
            return NotFound(new { message = $"Subscription '{id}' not found" });
        }
        
        return Ok(subscription);
    }
}
