using EmailService.Test.Utility.Fixtures.Hosting;

namespace EmailService.Test.Integration.Worker.Collections
{
    [CollectionDefinition(nameof(TestHostCollection))]
    public class TestHostCollection : ICollectionFixture<TestHostFixture>
    {
    }
}
