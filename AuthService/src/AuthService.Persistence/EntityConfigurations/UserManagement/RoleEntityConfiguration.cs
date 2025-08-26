using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Application.Validations.Constants.Entities.UserManagement;
using AuthService.Domain.Entities.UserManagement;

namespace AuthService.Persistence.EntityConfigurations.UserManagement
{
    public class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasIndex(r => r.Name)
                .IsUnique();

            builder.Property(r => r.Name)
                .HasMaxLength(RoleConstants.NameMaxLength);
        }
    }
}
