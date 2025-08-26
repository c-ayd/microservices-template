using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.Collections
{
    [CollectionDefinition(nameof(AppDbContextCollection))]
    public class AppDbContextCollection : ICollectionFixture<AppDbContextFixture>
    {
    }
}
