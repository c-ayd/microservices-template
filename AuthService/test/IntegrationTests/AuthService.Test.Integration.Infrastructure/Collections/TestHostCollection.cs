using AuthService.Test.Utility.Fixtures.Hosting;

namespace AuthService.Test.Integration.Infrastructure.Collections
{
    [CollectionDefinition(nameof(TestHostCollection))]
    public class TestHostCollection : ICollectionFixture<TestHostFixture>
    {
    }
}
