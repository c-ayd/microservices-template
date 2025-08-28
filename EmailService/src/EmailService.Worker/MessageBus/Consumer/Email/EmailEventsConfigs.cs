namespace EmailService.Worker.MessageBus.Consumer.Email
{
    public static class EmailEventsConfigs
    {
        public const string ExchangeName = "email.events";

        public const string DlxName = "email.dlx";
        public const string DlxRoutingKey = "email.poisoned";
    }
}
