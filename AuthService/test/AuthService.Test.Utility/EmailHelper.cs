namespace AuthService.Test.Utility
{
    public static class EmailHelper
    {
        public static void SetSendEmailEventResult(bool success)
        {
            AppDomain.CurrentDomain.SetData("SendEmailEventResult", success);
        }

        public static void SetUseMessageBus(bool useMessageBus)
        {
            AppDomain.CurrentDomain.SetData("UseMessageBus", useMessageBus);
        }
    }
}
