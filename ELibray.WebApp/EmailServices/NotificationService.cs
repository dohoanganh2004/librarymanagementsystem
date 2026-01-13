using ELibrary.WebApp.Models;

namespace ELibrary.WebApp.EmailServices
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;

        public NotificationService(IEmailService emailService)
        {
            _emailService = emailService;
        }

         public async Task NotifyAsync(NotificationMessage message)
        {
            await _emailService.SendEmailAsync(
            message.ToEmail,
            message.Subject,
            message.Body
        );
        }
    }
}
