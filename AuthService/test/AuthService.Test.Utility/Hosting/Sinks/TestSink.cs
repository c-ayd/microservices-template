using Cayd.AspNetCore.FlexLog.Logging;
using Cayd.AspNetCore.FlexLog.Sinks;

namespace AuthService.Test.Utility.Hosting.Sinks
{
    public class TestSink : FlexLogSink
    {
        public override async Task SaveLogsAsync(IReadOnlyList<FlexLogContext> buffer)
        {
        }
    }
}
