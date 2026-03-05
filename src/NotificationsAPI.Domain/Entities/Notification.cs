namespace NotificationsAPI.Domain.Entities;

/// <summary>
/// Entidade que representa uma notificação enviada
/// </summary>
public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsSent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum NotificationType
{
    WelcomeEmail = 1,
    PurchaseConfirmation = 2,
    PaymentDeclined = 3
}