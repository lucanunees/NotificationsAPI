namespace NotificationsAPI.Infrastructure.RabbitMQ
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// Nome da fila para eventos de criação de usuário
        /// </summary>
        public string UserCreatedQueue { get; set; } = "user-created-queue";

        /// <summary>
        /// Nome da fila para eventos de pagamento processado
        /// </summary>
        public string PaymentProcessedQueue { get; set; } = "payment-processed-queue";
    }
}
