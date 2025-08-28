using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AuthService.Infrastructure.MessageBus
{
    public class RabbitMqConnection : IAsyncDisposable
    {
        public IConnection? Connection { get; private set; }

        public RabbitMqConnection(string connectionString, ILoggerFactory loggerFactory)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(connectionString)
                };

                Connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                var logger = loggerFactory.CreateLogger(typeof(RabbitMqConnection));
                logger.LogCritical(exception, "Could not connect to RabbitMQ: " + exception.Message);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Connection != null)
            {
                await Connection.DisposeAsync();
            }
        }
    }
}
