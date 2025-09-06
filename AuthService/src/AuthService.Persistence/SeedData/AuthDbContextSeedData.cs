using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthService.Application.Policies;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.DbContexts;
using AuthService.Persistence.Settings;

namespace AuthService.Persistence.SeedData
{
    public static class AuthDbContextSeedData
    {
        public static async Task SeedDataAuthDbContext(this IServiceProvider services, IConfiguration configuration)
        {
            /**
             * NOTE: This seed data method is an example to show how to add default roles and email addresses.
             * Depending on specific needs, seeding data logic and how the default values are read can be changed.
             */

            await using var scope = services.CreateAsyncScope();
            var authDbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var seedDataSettings = configuration.GetSection(SeedDataAuthDbContextSettings.SettingsKey).Get<SeedDataAuthDbContextSettings>()!;

            await authDbContext.Database.EnsureCreatedAsync();

            await using var transaction = await authDbContext.Database.BeginTransactionAsync();
            try
            {
                var roles = await AddDefaultRoles(authDbContext);
                if (roles.Count == 0)
                    return;

                var users = await AddAdminAccounts(authDbContext, roles, seedDataSettings);
                if (users.Count == 0)
                    return;

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static async Task<List<Role>> AddDefaultRoles(AuthDbContext authDbContext)
        {
            var roles = new List<Role>();
            if (authDbContext.Roles.Any())
                return roles;

            roles.Add(new Role() { Name = AdminPolicy.RoleName });

            await authDbContext.AddRangeAsync(roles);
            await authDbContext.SaveChangesAsync();

            return roles;
        }

        private static async Task<List<User>> AddAdminAccounts(AuthDbContext authDbContext, List<Role> roles, SeedDataAuthDbContextSettings seedDataSettings)
        {
            var users = new List<User>();
            if (authDbContext.Users.Any())
                return users;

            var adminRole = roles
                .Where(r => r.Name == AdminPolicy.RoleName)
                .FirstOrDefault();

            if (adminRole == null)
                return users;

            foreach (var email in seedDataSettings.AdminEmails)
            {
                users.Add(new User()
                {
                    Email = email,
                    SecurityState = new SecurityState()
                    {
                        IsEmailVerified = true,
                    },
                    Roles = new List<Role>()
                    {
                        adminRole
                    }
                });
            }

            await authDbContext.AddRangeAsync(users);
            await authDbContext.SaveChangesAsync();

            return users;
        }
    }
}
