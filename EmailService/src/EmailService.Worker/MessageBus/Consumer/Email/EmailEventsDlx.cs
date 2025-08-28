using RabbitMQ.Client;

namespace EmailService.Worker.MessageBus.Consumer.Email
{
    public class EmailEventsDlx : RabbitMqDlxBase
    {
        public EmailEventsDlx(RabbitMqConnection connection, ILoggerFactory loggerFactory) 
            : base(connection, EmailEventsConfigs.DlxName, ExchangeType.Topic, EmailEventsConfigs.DlxRoutingKey, loggerFactory)
        {
        }
    }
}
