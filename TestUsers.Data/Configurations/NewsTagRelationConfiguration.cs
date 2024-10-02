using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class NewsTagRelationConfiguration : IEntityTypeConfiguration<NewsTagRelation>
{
    public void Configure(EntityTypeBuilder<NewsTagRelation> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.News)
            .WithMany(p => p.Tags)
            .HasForeignKey(p => p.NewsId);

        builder.HasOne(p => p.NewsTag)
            .WithMany(p => p.News)
            .HasForeignKey(p => p.NewsTagId);
    }
}