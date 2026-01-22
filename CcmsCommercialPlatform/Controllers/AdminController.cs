using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CcmsCommercialPlatform.Api.Data;
using CcmsCommercialPlatform.Api.Models.DTOs;
using CcmsCommercialPlatform.Api.Models.Enums;
using CcmsCommercialPlatform.Api.Services;

namespace CcmsCommercialPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IMeteredBillingService _meteredBillingService;
    
    private const string AdminPassword = "admin123";
    
    public AdminController(
        AppDbContext context,
        ISubscriptionService subscriptionService,
        IMeteredBillingService meteredBillingService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
        _meteredBillingService = meteredBillingService;
    }
    
    /// <summary>
    /// Verify admin password
    /// </summary>
    [HttpPost("verify")]
    public IActionResult VerifyPassword([FromBody] AdminVerifyRequest request)
    {
        if (request.Password == AdminPassword)
        {
            return Ok(new { valid = true });
        }
        
        return Unauthorized(new { valid = false, message = "Invalid password" });
    }
    
    /// <summary>
    /// Get all subscriptions (with pagination/filter)
    /// </summary>
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromHeader(Name = "X-Admin-Password")] string? password = null)
    {
        if (password != AdminPassword)
        {
            return Unauthorized(new { message = "Invalid admin password" });
        }
        
        var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync(page, pageSize, status);
        var total = await _context.Subscriptions.CountAsync();
        
        return Ok(new
        {
            data = subscriptions,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }
    
    /// <summary>
    /// Get all metered billing events sent
    /// </summary>
    [HttpGet("usage-events")]
    public async Task<IActionResult> GetUsageEvents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? subscriptionId = null,
        [FromHeader(Name = "X-Admin-Password")] string? password = null)
    {
        if (password != AdminPassword)
        {
            return Unauthorized(new { message = "Invalid admin password" });
        }
        
        var events = await _meteredBillingService.GetUsageEventsAsync(subscriptionId, page, pageSize);
        var total = await _context.UsageEvents.CountAsync();
        
        return Ok(new
        {
            data = events,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }
    
    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats(
        [FromHeader(Name = "X-Admin-Password")] string? password = null)
    {
        if (password != AdminPassword)
        {
            return Unauthorized(new { message = "Invalid admin password" });
        }
        
        var today = DateTime.UtcNow.Date;
        var thisMonth = new DateTime(today.Year, today.Month, 1);
        
        var activeSubscriptions = await _context.Subscriptions
            .Where(s => s.Status == SubscriptionStatus.Active)
            .Include(s => s.Plan)
            .ToListAsync();
        
        var mrr = activeSubscriptions.Sum(s => s.Plan?.MonthlyPrice ?? 0);
        
        var meteredRevenue = await _context.UsageEvents
            .Where(e => e.CreatedAt >= thisMonth)
            .SumAsync(e => e.Amount);
        
        var eventsToday = await _context.UsageEvents
            .Where(e => e.CreatedAt >= today)
            .CountAsync();
        
        var subscriptionsByPlan = activeSubscriptions
            .GroupBy(s => s.Plan?.Name ?? "Unknown")
            .ToDictionary(g => g.Key, g => g.Count());
        
        var stats = new AdminDashboardStats
        {
            TotalActiveSubscriptions = activeSubscriptions.Count,
            MonthlyRecurringRevenue = mrr,
            TotalMeteredRevenue = meteredRevenue,
            UsageEventsToday = eventsToday,
            SubscriptionsByPlan = subscriptionsByPlan
        };
        
        return Ok(stats);
    }
}

public class AdminVerifyRequest
{
    public string Password { get; set; } = string.Empty;
}
