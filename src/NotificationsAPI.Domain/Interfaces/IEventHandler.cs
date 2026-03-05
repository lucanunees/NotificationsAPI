namespace NotificationsAPI.Domain.Interfaces;

/// <summary>
/// Contrato genérico para handlers de eventos
/// </summary>
public interface IEventHandler<TEvent>
{
    /// <summary>
    /// Manipula um evento
    /// </summary>
    Task HandleAsync(TEvent @event);
}