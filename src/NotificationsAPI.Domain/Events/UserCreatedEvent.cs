namespace NotificationsAPI.Domain.Events;

/// <summary>
/// Evento disparado quando um usuário é criado em outro serviço
/// </summary>
public class UserCreatedEvent
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}