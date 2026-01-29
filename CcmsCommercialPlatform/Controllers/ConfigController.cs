using Microsoft.AspNetCore.Mvc;

namespace CcmsCommercialPlatform.Controllers;

/// <summary>
/// Provides public configuration for the frontend application.
/// Only non-sensitive configuration is exposed here.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(IConfiguration configuration, ILogger<ConfigController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Get public configuration for the frontend application.
    /// This includes Entra ID settings for SSO on the landing page.
    /// </summary>
    [HttpGet]
    public ActionResult<ClientConfig> GetConfig()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        
        var config = new ClientConfig
        {
            EntraId = new EntraIdConfig
            {
                ClientId = _configuration["EntraId:ClientId"] ?? "",
                Authority = _configuration["EntraId:Authority"] ?? "https://login.microsoftonline.com/common",
                RedirectUri = $"{baseUrl}{_configuration["EntraId:RedirectUri"] ?? "/azure-landing"}",
                PostLogoutRedirectUri = $"{baseUrl}{_configuration["EntraId:PostLogoutRedirectUri"] ?? "/"}"
            },
            IsDemo = _configuration.GetValue<bool>("IsDemo", true),
            ApplicationUrl = _configuration["ApplicationUrl"] ?? baseUrl
        };

        return Ok(config);
    }
}

/// <summary>
/// Public configuration for the frontend application.
/// </summary>
public class ClientConfig
{
    public EntraIdConfig EntraId { get; set; } = new();
    public bool IsDemo { get; set; }
    public string ApplicationUrl { get; set; } = "";
}

/// <summary>
/// Entra ID (Azure AD) configuration for SSO.
/// Only public information - no secrets.
/// </summary>
public class EntraIdConfig
{
    public string ClientId { get; set; } = "";
    public string Authority { get; set; } = "";
    public string RedirectUri { get; set; } = "";
    public string PostLogoutRedirectUri { get; set; } = "";
}
