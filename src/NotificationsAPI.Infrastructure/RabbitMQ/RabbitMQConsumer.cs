using NotificationsAPI.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NotificationsAPI.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Consumidor genérico do RabbitMQ (API v7)
    /// Desserializa mensagens JSON e delega ao handler correspondente
    /// </summary>
    public class RabbitMQConsumer<TEvent> : IDisposable where TEvent : class
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly string _queueName;
        private readonly IEventHandler<TEvent> _eventHandler;
        private readonly RabbitMQSettings _settings;

        public RabbitMQConsumer( RabbitMQSettings settings, string queueName, IEventHandler<TEvent> eventHandler)
        {
            _settings = settings;
            _queueName = queueName;
            _eventHandler = eventHandler;
        }

        /// <summary>
        /// Inicializa a conexão, declara a fila e começa a consumir mensagens
        /// </summary>
        public async Task StartConsumingAsync(CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            // v7: CreateConnectionAsync (tudo assíncrono)
            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // Declara a fila (idempotente)
            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            // Prefetch de 1 mensagem (fair dispatch)
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"📩 Mensagem recebida na fila '{_queueName}': {message}");

                try
                {
                    var @event = JsonSerializer.Deserialize<TEvent>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (@event is not null)
                    {
                        await _eventHandler.HandleAsync(@event);

                        // ACK manual — confirma processamento bem-sucedido
                        await _channel.BasicAckAsync(
                            deliveryTag: ea.DeliveryTag,
                            multiple: false,
                            cancellationToken: cancellationToken);

                        Console.WriteLine("✅ Mensagem processada e confirmada (ACK)");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Mensagem desserializada como null, rejeitando...");
                        await _channel.BasicNackAsync(
                            deliveryTag: ea.DeliveryTag,
                            multiple: false,
                            requeue: false,
                            cancellationToken: cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erro ao processar mensagem: {ex.Message}");

                    // Rejeita e recoloca na fila para retry
                    await _channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: cancellationToken);
                }
            };

            // autoAck: false → controle manual de ACK/NACK
            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);

            Console.WriteLine($"🐇 Consumidor iniciado na fila: '{_queueName}'");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            Console.WriteLine($"🔌 Consumidor da fila '{_queueName}' desconectado");
        }
    }
}
