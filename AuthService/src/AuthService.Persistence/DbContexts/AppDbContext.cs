using Microsoft.EntityFrameworkCore;
using System.Reflection;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.GlobalFilters;
using AuthService.Persistence.Interceptors;

namespace AuthService.Persistence.DbContexts
{
    public class AppDbContext : DbContext
    {
        // User management
        public DbSet<User> Users { get; set; }
        public DbSet<SecurityState> SecurityStates { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.ApplySoftDeleteFilter();

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new CreatedDateInterceptor());
            optionsBuilder.AddInterceptors(new UpdatedDateInterceptor());
            optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());

            base.OnConfiguring(optionsBuilder);
        }
    }
}
