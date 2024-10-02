using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class UserLanguageConfiguration : IEntityTypeConfiguration<UserLanguage>
{
    public void Configure(EntityTypeBuilder<UserLanguage> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DateLearn)
            .IsRequired();

        builder.HasOne(p => p.User)
            .WithMany(p => p.UserLanguages)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(p => p.Language)
            .WithMany(p => p.Users)
            .HasForeignKey(p => p.LanguageId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}