namespace NotificationsAPI.Domain.Interfaces;

using NotificationsAPI.Domain.Entities;

/// <summary>
/// Contrato para persistência de notificações
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Adiciona uma nova notificação
    /// </summary>
    Task AddAsync(Notification notification);

    /// <summary>
    /// Obtém uma notificação pelo ID
    /// </summary>
    Task<Notification?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtém todas as notificações de um usuário
    /// </summary>
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Atualiza uma notificação
    /// </summary>
    Task UpdateAsync(Notification notification);

    /// <summary>
    /// Salva as mudanças (commit)
    /// </summary>
    Task SaveChangesAsync();
}