using EmailService.Worker.MessageBus.Consumer.Email.Events.SendEmail;

namespace EmailService.Worker.Configurations
{
    public static partial class Configurations
    {
        public static async Task StartMessageBus(this IHost host)
        {
            // NOTE: Get all message bus events and start them
            var sendEmailEvent = host.Services.GetRequiredService<SendEmailEvent>();
            await sendEmailEvent.StartAsync();
        }
    }
}
