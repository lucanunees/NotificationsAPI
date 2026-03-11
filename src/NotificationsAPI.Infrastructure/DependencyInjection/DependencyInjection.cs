using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationsAPI.Application.DTOs;
using NotificationsAPI.Application.EventHandlers;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Domain.Interfaces;
using NotificationsAPI.Infrastructure.Email;
using NotificationsAPI.Infrastructure.Persistence;
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
            // 1. Configuração do DbContext com SQL Server
            services.AddDbContext<NotificationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ConnectionString")));

            // 2. Configuração do RabbitMQ (bind da seção "RabbitMQ" do appsettings.json)
            var rabbitSettings = new RabbitMQSettings();
            configuration.GetSection("RabbitMQ").Bind(rabbitSettings);
            services.AddSingleton(rabbitSettings);

            // 3. Serviços de infraestrutura
            services.AddSingleton<IEmailSender, EmailService>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            // 4. Use cases (Application layer)
            services.AddScoped<SendWelcomeEmailUseCase>();
            services.AddScoped<SendPurchaseConfirmationUseCase>();

            // 5. Event handlers (Application layer)
            services.AddScoped<IEventHandler<UserCreatedEventDto>, UserCreatedEventHandler>();
            services.AddScoped<IEventHandler<PaymentProcessedEventDto>, PaymentProcessedEventHandler>();

            // 6. Publicador RabbitMQ (uso futuro)
            services.AddSingleton<RabbitMQPublisher>();

            return services;
        }
    }
}
