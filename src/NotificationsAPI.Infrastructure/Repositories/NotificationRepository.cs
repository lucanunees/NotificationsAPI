using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Interfaces;

namespace NotificationsAPI.Infrastructure.Repositories
{

    /// <summary>
    /// Repositório em memória para notificações (substituir por EF Core em produção)
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {
        private static readonly List<Notification> _notifications = new();

        public Task AddAsync(Notification notification)
        {
            _notifications.Add(notification);
            Console.WriteLine($"💾 Notificação persistida: {notification.Id} | Tipo: {notification.Type} | Email: {notification.UserEmail}");
            return Task.CompletedTask;
        }

        public Task<Notification?> GetByIdAsync(Guid id)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == id);
            return Task.FromResult(notification);
        }

        public Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
        {
            var notifications = _notifications.Where(n => n.UserId == userId).AsEnumerable();
            return Task.FromResult(notifications);
        }

        public Task UpdateAsync(Notification notification)
        {
            var index = _notifications.FindIndex(n => n.Id == notification.Id);
            if (index >= 0)
                _notifications[index] = notification;

            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            // Em memória — sem operação. Com EF Core seria context.SaveChangesAsync()
            return Task.CompletedTask;
        }
    }
}
