namespace CcmsCommercialPlatform.Api.Models.Email;

public class MailSenderOptions
{
    public string SmtpServer { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;
    public string SenderName { get; set; } = "ComsignTrust CMS";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
}
