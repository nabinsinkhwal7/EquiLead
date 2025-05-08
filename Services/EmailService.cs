using MailKit.Net.Smtp;
using MimeKit;

namespace EquidCMS.Services;
public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var emailSettings = _config.GetSection("EmailSettings");
            string smtpServer = emailSettings["SMTPServer"];
            int smtpPort = int.Parse(emailSettings["SMTPPort"]);
            string senderEmail = emailSettings["SenderEmail"];
            string senderPassword = emailSettings["SenderPassword"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Equilead", senderEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };
            
            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, senderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendEmailWithBCCAsync(string toEmail,string[] bccEmails, string subject, string body)
    {
        try
        {
            var emailSettings = _config.GetSection("EmailSettings");
            string smtpServer = emailSettings["SMTPServer"];
            int smtpPort = int.Parse(emailSettings["SMTPPort"]);
            string senderEmail = emailSettings["SenderEmail"];
            string senderPassword = emailSettings["SenderPassword"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Equilead", senderEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            // Add all recipients to the Bcc list
            foreach (var email in bccEmails)
            {
                message.Bcc.Add(new MailboxAddress("", email));
            }

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, senderPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }


}

