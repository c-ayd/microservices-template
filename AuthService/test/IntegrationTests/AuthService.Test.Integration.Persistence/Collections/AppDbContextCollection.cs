using AuthService.Test.Utility.Fixtures.DbContexts;

namespace AuthService.Test.Integration.Persistence.Collections
{
    [CollectionDefinition(nameof(AuthDbContextCollection))]
    public class AuthDbContextCollection : ICollectionFixture<AuthDbContextFixture>
    {
    }
}
