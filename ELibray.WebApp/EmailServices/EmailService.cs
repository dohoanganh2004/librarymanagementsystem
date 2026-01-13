using ELibrary.WebApp.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace ELibrary.WebApp.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();

            
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));

            
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            
            var bodyBuilder = new BodyBuilder { HtmlBody = message };
            email.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                try
                {
                    
                    await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                    
                    await smtp.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                    
                    await smtp.SendAsync(email);
                }
                catch (Exception ex)
                {
                   
                     throw new InvalidOperationException("Lỗi gửi email: " + ex.Message, ex);
                }
                finally
                {
                    await smtp.DisconnectAsync(true);
                }
            }
        }
    }
}
