using AuthService.Test.Utility.Fixtures.Hosting;

namespace AuthService.Test.Integration.Api.Collections
{
    [CollectionDefinition(nameof(TestHostCollection))]
    public class TestHostCollection : ICollectionFixture<TestHostFixture>
    {
    }
}
