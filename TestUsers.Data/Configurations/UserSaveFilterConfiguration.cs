using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class UserSaveFilterConfiguration : IEntityTypeConfiguration<UserSaveFilter>
{
    public void Configure(EntityTypeBuilder<UserSaveFilter> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FilterName)
            .HasMaxLength(100);

        builder.Property(p => p.Search)
            .HasMaxLength(200);

        builder.Property(p => p.FromAmount)
            .HasPrecision(18, 4);

        builder.Property(p => p.ToAmount)
            .HasPrecision(18, 4);

        builder.Property(p => p.DateCreated);

        builder.HasOne(p => p.User)
            .WithMany(p => p.SaveFilters)
            .HasForeignKey(p => p.UserId);

        builder.HasOne(p => p.ProductCategory)
            .WithMany(p => p.UserSaveFilters)
            .HasForeignKey(p => p.CategoryId);
    }
}
