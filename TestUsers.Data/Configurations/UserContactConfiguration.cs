using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class UserContactConfiguration : IEntityTypeConfiguration<UserContact>
{
    public void Configure(EntityTypeBuilder<UserContact> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Value)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasOne(p => p.User)
            .WithMany(p => p.Contacts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}