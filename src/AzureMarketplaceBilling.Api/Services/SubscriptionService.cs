using Microsoft.EntityFrameworkCore;
using AzureMarketplaceBilling.Api.Data;
using AzureMarketplaceBilling.Api.Models;
using AzureMarketplaceBilling.Api.Models.DTOs;
using AzureMarketplaceBilling.Api.Models.Enums;

namespace AzureMarketplaceBilling.Api.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;
    
    public SubscriptionService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Subscription> CreateSubscriptionAsync(CreateSubscriptionRequest request)
    {
        var plan = await _context.Plans.FindAsync(request.PlanId);
        if (plan == null)
        {
            throw new ArgumentException($"Plan '{request.PlanId}' not found");
        }
        
        var now = DateTime.UtcNow;
        var billingPeriodStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var billingPeriodEnd = billingPeriodStart.AddMonths(1).AddDays(-1);
        
        var subscription = new Subscription
        {
            Id = Guid.NewGuid().ToString(),
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CompanyName = request.CompanyName,
            PlanId = request.PlanId,
            StartDate = now,
            Status = SubscriptionStatus.Active,
            CreatedAt = now,
            BillingPeriodStart = billingPeriodStart,
            BillingPeriodEnd = billingPeriodEnd
        };
        
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        
        // Reload with plan data
        return (await GetSubscriptionAsync(subscription.Id))!;
    }
    
    public async Task<Subscription?> GetSubscriptionAsync(string id)
    {
        return await _context.Subscriptions
            .Include(s => s.Plan)
            .ThenInclude(p => p!.Quotas)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public async Task<List<Subscription>> GetAllSubscriptionsAsync(int page = 1, int pageSize = 20, string? status = null)
    {
        var query = _context.Subscriptions
            .Include(s => s.Plan)
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SubscriptionStatus>(status, true, out var statusEnum))
        {
            query = query.Where(s => s.Status == statusEnum);
        }
        
        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<Subscription?> ChangePlanAsync(string subscriptionId, string newPlanId)
    {
        var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
        if (subscription == null)
        {
            return null;
        }
        
        var newPlan = await _context.Plans.FindAsync(newPlanId);
        if (newPlan == null)
        {
            throw new ArgumentException($"Plan '{newPlanId}' not found");
        }
        
        subscription.PlanId = newPlanId;
        await _context.SaveChangesAsync();
        
        return await GetSubscriptionAsync(subscriptionId);
    }
    
    public async Task<bool> CancelSubscriptionAsync(string subscriptionId)
    {
        var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
        if (subscription == null)
        {
            return false;
        }
        
        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.EndDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<Subscription?> StartNewBillingPeriodAsync(string subscriptionId)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.UsageRecords)
            .FirstOrDefaultAsync(s => s.Id == subscriptionId);
        
        if (subscription == null)
        {
            return null;
        }
        
        // Archive current usage records (keep them but mark as previous period)
        // In a real app, you might move these to a history table
        
        // Clear current period usage records
        var currentPeriodRecords = subscription.UsageRecords
            .Where(r => r.BillingPeriodStart == subscription.BillingPeriodStart)
            .ToList();
        
        // Reset the billing period
        var now = DateTime.UtcNow;
        subscription.BillingPeriodStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        subscription.BillingPeriodEnd = subscription.BillingPeriodStart.AddMonths(1).AddDays(-1);
        
        await _context.SaveChangesAsync();
        
        return await GetSubscriptionAsync(subscriptionId);
    }
}
