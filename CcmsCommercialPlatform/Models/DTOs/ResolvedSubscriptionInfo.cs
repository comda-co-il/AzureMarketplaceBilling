namespace CcmsCommercialPlatform.Api.Models.DTOs;

/// <summary>
/// Information returned after resolving an Azure Marketplace token
/// </summary>
public class ResolvedSubscriptionInfo
{
    public string SubscriptionId { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
    public string OfferId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    
    public PurchaserInfo Purchaser { get; set; } = new();
    public BeneficiaryInfo Beneficiary { get; set; } = new();
    
    /// <summary>
    /// Internal ID of the created MarketplaceSubscription record
    /// </summary>
    public int MarketplaceSubscriptionId { get; set; }
}

public class PurchaserInfo
{
    public string EmailId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ObjectId { get; set; } = string.Empty;
}

public class BeneficiaryInfo
{
    public string EmailId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ObjectId { get; set; } = string.Empty;
}
