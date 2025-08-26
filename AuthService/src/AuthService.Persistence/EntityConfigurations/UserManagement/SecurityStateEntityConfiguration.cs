using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.Generators;

namespace AuthService.Persistence.EntityConfigurations.UserManagement
{
    public class SecurityStateEntityConfiguration : IEntityTypeConfiguration<SecurityState>
    {
        public void Configure(EntityTypeBuilder<SecurityState> builder)
        {
            builder.Property(ss => ss.Id)
                .HasValueGenerator<GuidIdGenerator>();
        }
    }
}
