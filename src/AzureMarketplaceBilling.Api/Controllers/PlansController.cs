using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AzureMarketplaceBilling.Api.Data;
using AzureMarketplaceBilling.Api.Models;

namespace AzureMarketplaceBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public PlansController(AppDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Get all available plans with their quotas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Plan>>> GetPlans()
    {
        var plans = await _context.Plans
            .Include(p => p.Quotas)
            .OrderBy(p => p.MonthlyPrice)
            .ToListAsync();
        
        return Ok(plans);
    }
    
    /// <summary>
    /// Get a specific plan by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Plan>> GetPlan(string id)
    {
        var plan = await _context.Plans
            .Include(p => p.Quotas)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (plan == null)
        {
            return NotFound(new { message = $"Plan '{id}' not found" });
        }
        
        return Ok(plan);
    }
}
