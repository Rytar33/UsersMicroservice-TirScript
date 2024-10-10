using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DateCreated);

        builder.HasOne(p => p.Role)
            .WithMany(p => p.UserRoles)
            .HasForeignKey(p => p.RoleId);

        builder.HasOne(p => p.User)
            .WithMany(p => p.UserRoles)
            .HasForeignKey(p => p.UserId);
    }
}