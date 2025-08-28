namespace AuthService.Infrastructure.MessageBus.Publisher.Email
{
    public static class EmailEventsConfigs
    {
        public const string ExchangeName = "email.events";

        public const string SendEmailRoutingKey = "email.send";
    }
}
