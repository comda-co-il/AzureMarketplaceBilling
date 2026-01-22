using System.ComponentModel.DataAnnotations;

namespace CcmsCommercialPlatform.Api.Models;

public class Plan
{
    [Key]
    public string Id { get; set; } = string.Empty;  // "starter", "professional", "enterprise"
    
    [Required]
    public string Name { get; set; } = string.Empty;  // "Starter", "Professional", "Enterprise"
    
    public string Description { get; set; } = string.Empty;
    
    public decimal MonthlyPrice { get; set; }  // 299, 799, 1999
    
    public bool IsPopular { get; set; }  // Highlight "Professional"
    
    public List<PlanQuota> Quotas { get; set; } = new();
}
