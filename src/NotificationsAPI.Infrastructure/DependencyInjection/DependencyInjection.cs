using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationsAPI.Application.DTOs;
using NotificationsAPI.Application.EventHandlers;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Domain.Interfaces;
using NotificationsAPI.Infrastructure.Email;
using NotificationsAPI.Infrastructure.RabbitMQ;
using NotificationsAPI.Infrastructure.Repositories;

namespace NotificationsAPI.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Extensão para registrar todos os serviços no container de DI
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            // 1. Configuração do RabbitMQ (bind da seção "RabbitMQ" do appsettings.json)
            var rabbitSettings = new RabbitMQSettings();
            configuration.GetSection("RabbitMQ").Bind(rabbitSettings);
            services.AddSingleton(rabbitSettings);

            // 2. Serviços de infraestrutura
            services.AddSingleton<IEmailSender, EmailService>();
            services.AddSingleton<INotificationRepository, NotificationRepository>();

            // 3. Use cases (Application layer)
            services.AddScoped<SendWelcomeEmailUseCase>();
            services.AddScoped<SendPurchaseConfirmationUseCase>();

            // 4. Event handlers (Application layer)
            services.AddScoped<IEventHandler<UserCreatedEventDto>, UserCreatedEventHandler>();
            services.AddScoped<IEventHandler<PaymentProcessedEventDto>, PaymentProcessedEventHandler>();

            // 5. Publicador RabbitMQ (uso futuro)
            services.AddSingleton<RabbitMQPublisher>();

            return services;
        }
    }
}
