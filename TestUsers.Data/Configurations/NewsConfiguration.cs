using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.DateCreated);

        builder.HasMany(p => p.Tags)
            .WithOne(p => p.News)
            .HasForeignKey(p => p.NewsId);

        builder.HasOne(p => p.Author)
            .WithMany(p => p.NewsCreated)
            .HasForeignKey(p => p.AuthorId);
    }
}