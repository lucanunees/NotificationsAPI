namespace NotificationsAPI.Application.DTOs;

/// <summary>
/// DTO para o evento de pagamento processado
/// </summary>
public class PaymentProcessedEventDto
{
    public Guid PaymentId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty; // "Approved" ou "Declined"
    public DateTime ProcessedAt { get; set; }
}