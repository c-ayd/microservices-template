using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Application.Validations.Constants.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.Generators;

namespace AuthService.Persistence.EntityConfigurations.UserManagement
{
    public class LoginEntityConfiguration : IEntityTypeConfiguration<Login>
    {
        public void Configure(EntityTypeBuilder<Login> builder)
        {
            builder.Property(l => l.Id)
                .HasValueGenerator<GuidIdGenerator>();

            builder.HasIndex(l => l.RefreshTokenHashed)
                .IsUnique();

            builder.Property(l => l.DeviceInfo)
                .HasMaxLength(LoginConstants.DeviceInfoMaxLength);
        }
    }
}
