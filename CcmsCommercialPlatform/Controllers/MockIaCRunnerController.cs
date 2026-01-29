using Microsoft.AspNetCore.Mvc;
using CcmsCommercialPlatform.Api.Models.DTOs;

namespace CcmsCommercialPlatform.Api.Controllers;

/// <summary>
/// Mock controller for Claude IaC Runner API - FOR DEVELOPMENT/TESTING ONLY
/// This simulates the real IaC Runner service response
/// Remove or disable this controller when connecting to the real service
/// </summary>
[ApiController]
[Route("api/mock/iac-runner")]
public class MockIaCRunnerController : ControllerBase
{
    private readonly ILogger<MockIaCRunnerController> _logger;
    private readonly IConfiguration _configuration;
    
    public MockIaCRunnerController(ILogger<MockIaCRunnerController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    
    /// <summary>
    /// Mock endpoint for infrastructure provisioning
    /// Returns a successful response simulating CCMS container deployment
    /// </summary>
    [HttpPost("provision")]
    public IActionResult Provision([FromBody] IaCRunnerRequest request)
    {
        _logger.LogInformation(
            "[MOCK] IaC Runner provision request received for subscription {SubscriptionId}, Company: {Company}",
            request.SubscriptionId,
            request.Customer?.Company);
        
        // Log the full request for debugging
        _logger.LogDebug("[MOCK] Full request: {@Request}", request);
        
        // Simulate processing delay (optional - uncomment to test timeout handling)
        // await Task.Delay(2000);
        
        // Return successful response
        var response = new IaCRunnerResponse
        {
            Id = $"deploy-{Guid.NewGuid():N}",
            Success = true,
            Message = $"CCMS container provisioning initiated successfully for {request.Customer?.Company ?? "Unknown"}. Deployment ID: deploy-{DateTime.UtcNow:yyyyMMddHHmmss}"
        };
        
        _logger.LogInformation("[MOCK] Returning success response with deployment ID: {DeploymentId}", response.Id);
        
        return Ok(response);
    }
    
    /// <summary>
    /// Mock endpoint that returns a failure response - for testing error handling
    /// </summary>
    [HttpPost("provision-fail")]
    public IActionResult ProvisionFail([FromBody] IaCRunnerRequest request)
    {
        _logger.LogWarning(
            "[MOCK] IaC Runner provision FAILURE simulation for subscription {SubscriptionId}",
            request.SubscriptionId);
        
        var response = new IaCRunnerResponse
        {
            Id = string.Empty,
            Success = false,
            Message = "Simulated failure: Unable to provision infrastructure. Please try again later."
        };
        
        return Ok(response);
    }
}
