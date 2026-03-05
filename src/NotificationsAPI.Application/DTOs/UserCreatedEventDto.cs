namespace NotificationsAPI.Application.DTOs;

/// <summary>
/// DTO para o evento de criação de usuário
/// </summary>
public class UserCreatedEventDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}