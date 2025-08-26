using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain.Entities.UserManagement;
using AuthService.Persistence.Generators;

namespace AuthService.Persistence.EntityConfigurations.UserManagement
{
    public class TokenEntityConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.Property(t => t.Id)
                .HasValueGenerator<GuidIdGenerator>();

            builder.HasIndex(t => t.ValueHashed)
                .IsUnique();
        }
    }
}
