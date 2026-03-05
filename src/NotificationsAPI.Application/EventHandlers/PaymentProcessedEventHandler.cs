namespace NotificationsAPI.Application.EventHandlers;

using NotificationsAPI.Application.DTOs;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Domain.Interfaces;

/// <summary>
/// Handler para o evento PaymentProcessedEvent
/// Consome mensagens do RabbitMQ e executa o use case de envio de e-mail de confirmação
/// </summary>
public class PaymentProcessedEventHandler : IEventHandler<PaymentProcessedEventDto>
{
    private readonly SendPurchaseConfirmationUseCase _sendPurchaseConfirmationUseCase;

    public PaymentProcessedEventHandler(SendPurchaseConfirmationUseCase sendPurchaseConfirmationUseCase)
    {
        _sendPurchaseConfirmationUseCase = sendPurchaseConfirmationUseCase;
    }

    /// <summary>
    /// Manipula o evento de pagamento processado
    /// </summary>
    public async Task HandleAsync(PaymentProcessedEventDto @event)
    {
        Console.WriteLine($"📬 Evento recebido: PaymentProcessedEvent com status '{@event.Status}'");

        // Apenas envia e-mail se o pagamento foi aprovado
        if (@event.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
        {
            await _sendPurchaseConfirmationUseCase.ExecuteAsync(
                @event.PaymentId,
                @event.UserId,
                @event.UserEmail,
                @event.Amount);
        }
        else
        {
            Console.WriteLine($"⏭️  Pagamento recusado - nenhum e-mail enviado");
        }
    }
}