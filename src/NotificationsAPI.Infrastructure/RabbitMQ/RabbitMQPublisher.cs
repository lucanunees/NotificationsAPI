using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace NotificationsAPI.Infrastructure.RabbitMQ
{

    /// <summary>
    /// Publicador genérico de mensagens no RabbitMQ (uso futuro)
    /// </summary>
    public class RabbitMQPublisher : IAsyncDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly RabbitMQSettings _settings;
        private bool _initialized;

        public RabbitMQPublisher(RabbitMQSettings settings)
        {
            _settings = settings;
        }

        private async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
        {
            if (_initialized) return;

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            _initialized = true;
        }

        /// <summary>
        /// Publica uma mensagem em uma fila específica
        /// </summary>
        public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
        {
            await EnsureInitializedAsync(cancellationToken);

            await _channel!.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            Console.WriteLine($"📤 Mensagem publicada na fila '{queueName}': {json}");
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null) await _channel.DisposeAsync();
            if (_connection is not null) await _connection.DisposeAsync();
        }
    }
}
