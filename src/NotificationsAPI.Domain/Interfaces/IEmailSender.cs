namespace NotificationsAPI.Domain.Interfaces;

/// <summary>
/// Contrato para implementação de envio de e-mails
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Envia um e-mail (simula enviando log no console)
    /// </summary>
    Task SendAsync(string toEmail, string subject, string body);
}