using NotificationsAPI.Application.DTOs;
using NotificationsAPI.Domain.Interfaces;
using NotificationsAPI.Infrastructure.RabbitMQ;

namespace NotificationsAPI.HostedServices
{
    public class RabbitMQConsumerHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _settings;
        private RabbitMQConsumer<UserCreatedEventDto>? _userCreatedConsumer;
        private RabbitMQConsumer<PaymentProcessedEventDto>? _paymentProcessedConsumer;

        public RabbitMQConsumerHostedService(
            IServiceProvider serviceProvider,
            RabbitMQSettings settings)
        {
            _serviceProvider = serviceProvider;
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("🚀 Iniciando consumidores RabbitMQ...");

            try
            {
                // Cria um scope para resolver serviços Scoped (handlers e use cases)
                var scope = _serviceProvider.CreateScope();

                var userCreatedHandler = scope.ServiceProvider
                    .GetRequiredService<IEventHandler<UserCreatedEventDto>>();

                var paymentProcessedHandler = scope.ServiceProvider
                    .GetRequiredService<IEventHandler<PaymentProcessedEventDto>>();

                // Consumidor 1: fila de criação de usuário → e-mail de boas-vindas
                _userCreatedConsumer = new RabbitMQConsumer<UserCreatedEventDto>(
                    _settings,
                    _settings.UserCreatedQueue,
                    userCreatedHandler);
                await _userCreatedConsumer.StartConsumingAsync(stoppingToken);

                // Consumidor 2: fila de pagamento processado → e-mail de confirmação (se Approved)
                _paymentProcessedConsumer = new RabbitMQConsumer<PaymentProcessedEventDto>(
                    _settings,
                    _settings.PaymentProcessedQueue,
                    paymentProcessedHandler);
                await _paymentProcessedConsumer.StartConsumingAsync(stoppingToken);

                Console.WriteLine("✅ Todos os consumidores RabbitMQ estão ativos!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao iniciar consumidores RabbitMQ: {ex.Message}");
                Console.WriteLine("⚠️  Verifique se o RabbitMQ está rodando e acessível.");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("🛑 Parando consumidores RabbitMQ...");
            _userCreatedConsumer?.Dispose();
            _paymentProcessedConsumer?.Dispose();
            return base.StopAsync(cancellationToken);
        }
    }
}
