namespace NotificationsAPI.Application.UseCases;

using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Interfaces;

/// <summary>
/// Use case para enviar e-mail de boas-vindas
/// </summary>
public class SendWelcomeEmailUseCase
{
    private readonly IEmailSender _emailSender;
    private readonly INotificationRepository _notificationRepository;

    public SendWelcomeEmailUseCase(
        IEmailSender emailSender,
        INotificationRepository notificationRepository)
    {
        _emailSender = emailSender;
        _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Executa o envio do e-mail de boas-vindas
    /// </summary>
    public async Task ExecuteAsync(Guid userId, string userName, string userEmail)
    {
        try
        {
            var subject = "Bem-vindo ao nosso serviço!";
            var body = $"Olá {userName}!\n\nSuas credenciais foram criadas com sucesso. " +
                      "Bem-vindo ao nosso serviço! 🎉";

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
                Type = NotificationType.WelcomeEmail,
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
            Console.WriteLine($"❌ Erro ao enviar e-mail de boas-vindas: {ex.Message}");
            throw;
        }
    }
}