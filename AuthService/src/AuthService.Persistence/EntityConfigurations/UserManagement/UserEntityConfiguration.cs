using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Application.Validations.Constants.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.Converters;
using AuthService.Persistence.Generators;

namespace AuthService.Persistence.EntityConfigurations.UserManagement
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Id)
                .HasValueGenerator<GuidIdGenerator>();

            builder.HasIndex(u => u.Email);

            builder.Property(u => u.Email)
                .HasMaxLength(UserConstants.EmailMaxLength)
                .HasConversion<ToLowerConverter>();

            builder.Property(u => u.NewEmail)
                .HasMaxLength(UserConstants.EmailMaxLength)
                .HasConversion<ToLowerConverter>();

            // Relationships
            builder.HasOne(u => u.SecurityState)
                .WithOne()
                .HasForeignKey<SecurityState>(ss => ss.UserId);

            builder.HasMany(u => u.Roles)
                .WithMany(r => r.Users);

            builder.HasMany(u => u.Logins)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);

            builder.HasMany(u => u.Tokens)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
