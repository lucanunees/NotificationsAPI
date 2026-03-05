namespace NotificationsAPI.Application.UseCases;

using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Interfaces;

/// <summary>
/// Use case para enviar e-mail de confirmação de compra
/// </summary>
public class SendPurchaseConfirmationUseCase
{
    private readonly IEmailSender _emailSender;
    private readonly INotificationRepository _notificationRepository;

    public SendPurchaseConfirmationUseCase(
        IEmailSender emailSender,
        INotificationRepository notificationRepository)
    {
        _emailSender = emailSender;
        _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Executa o envio do e-mail de confirmação de compra
    /// </summary>
    public async Task ExecuteAsync(Guid paymentId, Guid userId, string userEmail, decimal amount)
    {
        try
        {
            var subject = "Compra confirmada!";
            var body = $"Olá!\n\nSua compra de R$ {amount:F2} foi confirmada com sucesso! 🎊\n" +
                      $"ID do Pagamento: {paymentId}";

            // Envia o e-mail (simula no console)
            await _emailSender.SendAsync(userEmail, subject, body);

            // Cria registro de notificação
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UserEmail = userEmail,
                Subject = subject,
                Body = body,
                Type = NotificationType.PurchaseConfirmation,
                IsSent = true,
                CreatedAt = DateTime.UtcNow,
                SentAt = DateTime.UtcNow
            };

            // Persiste a notificação
            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log de erro (será melhorado com Serilog)
            Console.WriteLine($"❌ Erro ao enviar e-mail de confirmação: {ex.Message}");
            throw;
        }
    }
}