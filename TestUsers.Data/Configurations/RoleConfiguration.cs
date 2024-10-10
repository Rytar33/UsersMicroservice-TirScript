using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DateCreated);

        builder.Property(p => p.Name)
            .HasMaxLength(100);

        builder.HasMany(p => p.UserRoles)
            .WithOne(p => p.Role)
            .HasForeignKey(p => p.RoleId);
    }
}