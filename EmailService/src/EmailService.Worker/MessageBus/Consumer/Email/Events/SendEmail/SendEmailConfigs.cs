namespace EmailService.Worker.MessageBus.Consumer.Email.Events.SendEmail
{
    public static class SendEmailConfigs
    {
        public const string RoutingKey = "email.send";
        public const string QueueName = "email.email-service.send";
    }
}
