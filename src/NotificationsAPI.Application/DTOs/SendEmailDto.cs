namespace NotificationsAPI.Application.DTOs;

/// <summary>
/// DTO para requisição de envio de e-mail
/// </summary>
public class SendEmailDto
{
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}