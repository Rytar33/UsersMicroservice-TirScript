using Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace Dal.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(152);

        builder.Property(p => p.PasswordHash)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(64);

        builder.Property(p => p.DateRegister)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasDefaultValue(EnumUserStatus.NotConfirmed)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.RecoveryToken)
            .HasMaxLength(6);
    }
}