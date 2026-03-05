using NotificationsAPI.Domain.Interfaces;

namespace NotificationsAPI.Infrastructure.Email
{
    public class EmailService : IEmailSender
    {
        public Task SendAsync(string toEmail, string subject, string body)
        {
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("📧 E-MAIL ENVIADO (simulação)");
            Console.WriteLine($"   Para: {toEmail}");
            Console.WriteLine($"   Assunto: {subject}");
            Console.WriteLine($"   Corpo: {body}");
            Console.WriteLine("═══════════════════════════════════════════");

            return Task.CompletedTask;
        }
    }
}
