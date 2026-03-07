// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using MailKit.Net.Smtp;
using MimeKit;

namespace Admin.NET.Core.Service;

/// <summary>
/// System email sending service 🧩
/// </summary>
[ApiDescriptionSettings(Order = 370)]
public class SysEmailService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysTenant> _sysTenantRep;
    private readonly EmailOptions _emailOptions;

    public SysEmailService(IOptions<EmailOptions> emailOptions, SqlSugarRepository<SysTenant> sysTenantRep)
    {
        _emailOptions = emailOptions.Value;
        _sysTenantRep = sysTenantRep;
    }

    /// <summary>
    /// Send email 📧
    /// </summary>
    /// <param name="content"></param>
    /// <param name="title"></param>
    /// <param name="toEmail"></param>
    /// <returns></returns>
    [DisplayName("Send email")]
    public async Task SendEmail([Required] string content, string title = "", string toEmail = "")
    {
        long.TryParse(App.User?.FindFirst(ClaimConst.TenantId)?.Value ?? SqlSugarConst.DefaultTenantId.ToString(), out var tenantId);
        var webTitle = (await _sysTenantRep.GetFirstAsync(u => u.Id == tenantId))?.Title;
        title = string.IsNullOrWhiteSpace(title) ? $"{webTitle} System Email" : title;
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_emailOptions.DefaultFromEmail, _emailOptions.DefaultFromEmail));

        message.To.Add(string.IsNullOrWhiteSpace(toEmail)
            ? new MailboxAddress(_emailOptions.DefaultToEmail, _emailOptions.DefaultToEmail)
            : new MailboxAddress(toEmail, toEmail));

        message.Subject = title;
        message.Body = new TextPart("html")
        {
            Text = content
        };

        using var client = new SmtpClient();
        client.Connect(_emailOptions.Host, _emailOptions.Port, _emailOptions.EnableSsl);
        client.Authenticate(_emailOptions.UserName, _emailOptions.Password);
        client.Send(message);
        client.Disconnect(true);

        await Task.CompletedTask;
    }
}