using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.SessionId);

        builder.Property(p => p.DateCreated);

        builder.HasOne(p => p.User)
            .WithMany(p => p.Sessions)
            .HasForeignKey(p => p.UserId);
    }
}
