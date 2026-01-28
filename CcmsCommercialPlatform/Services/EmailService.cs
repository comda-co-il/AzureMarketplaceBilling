using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using CcmsCommercialPlatform.Api.Models;
using CcmsCommercialPlatform.Api.Models.Email;
using Microsoft.Extensions.Options;

namespace CcmsCommercialPlatform.Api.Services;

public class EmailService : IEmailService
{
    private readonly MailSenderOptions _mailOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<MailSenderOptions> mailOptions, ILogger<EmailService> logger)
    {
        _mailOptions = mailOptions.Value;
        _logger = logger;
    }

    public async Task<bool> SendWelcomeEmailAsync(MarketplaceSubscription subscription)
    {
        if (string.IsNullOrEmpty(subscription.CustomerEmail))
        {
            _logger.LogWarning("Cannot send welcome email - no customer email for subscription {Id}", subscription.Id);
            return false;
        }

        var messageContent = new MailMessageContent
        {
            Subject = "Welcome to ComsignTrust CMS - Your Subscription is Active!",
            Body = GenerateWelcomeEmailBody(subscription)
        };

        var mailInfo = new MailInfo
        {
            To = [subscription.CustomerEmail],
            MessageContent = messageContent
        };

        return await SendEmailAsync(mailInfo);
    }

    public async Task<bool> SendSubscriptionConfirmationAsync(MarketplaceSubscription subscription)
    {
        if (string.IsNullOrEmpty(subscription.CustomerEmail))
        {
            _logger.LogWarning("Cannot send confirmation email - no customer email for subscription {Id}", subscription.Id);
            return false;
        }

        var messageContent = new MailMessageContent
        {
            Subject = "Your ComsignTrust CMS Subscription Confirmation",
            Body = GenerateConfirmationEmailBody(subscription)
        };

        var mailInfo = new MailInfo
        {
            To = [subscription.CustomerEmail],
            MessageContent = messageContent
        };

        return await SendEmailAsync(mailInfo);
    }

    private async Task<bool> SendEmailAsync(MailInfo mailInfo)
    {
        if (string.IsNullOrEmpty(_mailOptions.SmtpServer) || string.IsNullOrEmpty(_mailOptions.SenderAddress))
        {
            _logger.LogWarning("Email not configured - SmtpServer or SenderAddress is missing");
            return false;
        }

        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_mailOptions.SenderName, _mailOptions.SenderAddress));

            foreach (var recipient in mailInfo.To)
            {
                mimeMessage.To.Add(new MailboxAddress("", recipient));
            }

            mimeMessage.Subject = mailInfo.MessageContent?.Subject ?? "";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = mailInfo.MessageContent?.Body ?? ""
            };
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = new SmtpClient();
            
            await smtpClient.ConnectAsync(
                _mailOptions.SmtpServer, 
                _mailOptions.SmtpPort, 
                SecureSocketOptions.Auto);

            if (!string.IsNullOrEmpty(_mailOptions.Username) && !string.IsNullOrEmpty(_mailOptions.Password))
            {
                await smtpClient.AuthenticateAsync(_mailOptions.Username, _mailOptions.Password);
            }

            await smtpClient.SendAsync(mimeMessage);
            await smtpClient.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", mailInfo.To));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", mailInfo.To));
            return false;
        }
    }

    private static string GenerateWelcomeEmailBody(MarketplaceSubscription subscription)
    {
        var customerName = !string.IsNullOrEmpty(subscription.CustomerName) 
            ? subscription.CustomerName 
            : "Valued Customer";
        
        var companyName = !string.IsNullOrEmpty(subscription.CompanyName) 
            ? subscription.CompanyName 
            : "your organization";

        var featuresHtml = "";
        if (subscription.FeatureSelections?.Any() == true)
        {
            var featuresList = string.Join("", subscription.FeatureSelections
                .Where(f => f.IsEnabled)
                .Select(f => $"<li><strong>{f.FeatureName}</strong> - {f.Quantity:N0} tokens</li>"));
            
            if (!string.IsNullOrEmpty(featuresList))
            {
                featuresHtml = $@"
                <h3 style=""color: #006e95; margin-top: 20px;"">Your Selected Token Packages:</h3>
                <ul style=""line-height: 1.8;"">
                    {featuresList}
                </ul>";
            }
        }

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
</head>
<body style=""font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: linear-gradient(135deg, #006e95 0%, #004d6e 100%); padding: 30px; text-align: center; border-radius: 8px 8px 0 0;"">
        <h1 style=""color: #ffffff; margin: 0; font-size: 28px;"">🔐 ComsignTrust CMS</h1>
        <p style=""color: #e1faff; margin: 10px 0 0 0; font-size: 16px;"">Credential Management System</p>
    </div>
    
    <div style=""background: #ffffff; padding: 30px; border: 1px solid #e0e0e0; border-top: none;"">
        <h2 style=""color: #006e95; margin-top: 0;"">Welcome, {customerName}! 🎉</h2>
        
        <p>Thank you for subscribing to <strong>ComsignTrust CMS</strong>! We're excited to have {companyName} on board.</p>
        
        <p>Your subscription has been successfully activated and you're ready to start managing your credentials securely.</p>
        
        <div style=""background: #e1faff; padding: 20px; border-radius: 8px; margin: 20px 0;"">
            <h3 style=""color: #006e95; margin-top: 0;"">Subscription Details:</h3>
            <table style=""width: 100%; border-collapse: collapse;"">
                <tr>
                    <td style=""padding: 8px 0; color: #666;"">Subscription ID:</td>
                    <td style=""padding: 8px 0; font-weight: bold;"">{subscription.AzureSubscriptionId}</td>
                </tr>
                <tr>
                    <td style=""padding: 8px 0; color: #666;"">Plan:</td>
                    <td style=""padding: 8px 0; font-weight: bold;"">{subscription.PlanId}</td>
                </tr>
                <tr>
                    <td style=""padding: 8px 0; color: #666;"">Company:</td>
                    <td style=""padding: 8px 0; font-weight: bold;"">{companyName}</td>
                </tr>
                <tr>
                    <td style=""padding: 8px 0; color: #666;"">Activated:</td>
                    <td style=""padding: 8px 0; font-weight: bold;"">{DateTime.UtcNow:MMMM dd, yyyy}</td>
                </tr>
            </table>
        </div>
        
        {featuresHtml}
        
        <h3 style=""color: #006e95; margin-top: 20px;"">Getting Started:</h3>
        <ol style=""line-height: 2;"">
            <li>Access your dashboard through the Azure Portal</li>
            <li>Configure your credential management settings</li>
            <li>Start issuing and managing tokens for your organization</li>
            <li>Monitor your usage in real-time</li>
        </ol>
        
        <div style=""background: #f8f8f8; padding: 20px; border-radius: 8px; margin: 20px 0; text-align: center;"">
            <p style=""margin: 0 0 15px 0;"">Need help getting started?</p>
            <a href=""mailto:support@comsigntrust.com"" style=""background: #006e95; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; font-weight: bold;"">Contact Support</a>
        </div>
    </div>
    
    <div style=""background: #f5f5f5; padding: 20px; text-align: center; border-radius: 0 0 8px 8px; border: 1px solid #e0e0e0; border-top: none;"">
        <p style=""margin: 0; color: #666; font-size: 14px;"">
            © {DateTime.UtcNow.Year} ComsignTrust. All rights reserved.<br>
            <a href=""mailto:support@comsigntrust.com"" style=""color: #006e95;"">support@comsigntrust.com</a>
        </p>
    </div>
</body>
</html>";
    }

    private static string GenerateConfirmationEmailBody(MarketplaceSubscription subscription)
    {
        var customerName = !string.IsNullOrEmpty(subscription.CustomerName) 
            ? subscription.CustomerName 
            : "Valued Customer";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
</head>
<body style=""font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: #006e95; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;"">
        <h1 style=""color: #ffffff; margin: 0;"">🔐 ComsignTrust CMS</h1>
    </div>
    
    <div style=""background: #ffffff; padding: 30px; border: 1px solid #e0e0e0; border-top: none; border-radius: 0 0 8px 8px;"">
        <h2 style=""color: #006e95;"">Subscription Confirmed</h2>
        <p>Dear {customerName},</p>
        <p>This email confirms that your ComsignTrust CMS subscription has been processed successfully.</p>
        <p>Subscription ID: <strong>{subscription.AzureSubscriptionId}</strong></p>
        <p>If you have any questions, please don't hesitate to contact us.</p>
        <p>Best regards,<br>The ComsignTrust Team</p>
    </div>
</body>
</html>";
    }
}
