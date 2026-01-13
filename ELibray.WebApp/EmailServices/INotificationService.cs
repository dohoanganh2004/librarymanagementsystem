using ELibrary.WebApp.Models;

namespace ELibrary.WebApp.EmailServices
{
    public interface INotificationService
    {
        Task NotifyAsync(NotificationMessage message);
    }
}
