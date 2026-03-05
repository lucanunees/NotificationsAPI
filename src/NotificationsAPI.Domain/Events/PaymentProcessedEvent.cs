namespace NotificationsAPI.Domain.Events;

/// <summary>
/// Evento disparado quando um pagamento é processado
/// </summary>
public class PaymentProcessedEvent
{
    public Guid PaymentId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty; // "Approved" ou "Declined"
    public DateTime ProcessedAt { get; set; }
}