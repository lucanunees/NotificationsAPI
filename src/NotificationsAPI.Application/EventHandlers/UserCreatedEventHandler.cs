namespace NotificationsAPI.Application.EventHandlers;

using NotificationsAPI.Application.DTOs;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Domain.Interfaces;

/// <summary>
/// Handler para o evento UserCreatedEvent
/// Consome mensagens do RabbitMQ e executa o use case de envio de e-mail de boas-vindas
/// </summary>
public class UserCreatedEventHandler : IEventHandler<UserCreatedEventDto>
{
    private readonly SendWelcomeEmailUseCase _sendWelcomeEmailUseCase;

    public UserCreatedEventHandler(SendWelcomeEmailUseCase sendWelcomeEmailUseCase)
    {
        _sendWelcomeEmailUseCase = sendWelcomeEmailUseCase;
    }

    /// <summary>
    /// Manipula o evento de criação de usuário
    /// </summary>
    public async Task HandleAsync(UserCreatedEventDto @event)
    {
        Console.WriteLine($"📬 Evento recebido: UserCreatedEvent para {{{@event.UserEmail}}}");

        await _sendWelcomeEmailUseCase.ExecuteAsync(
            @event.UserId,
            @event.UserName,
            @event.UserEmail);
    }
}