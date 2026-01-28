namespace CcmsCommercialPlatform.Api.Models.Email;

public class MailInfo
{
    public IEnumerable<string> To { get; set; } = [];
    public MailMessageContent? MessageContent { get; set; }
}
