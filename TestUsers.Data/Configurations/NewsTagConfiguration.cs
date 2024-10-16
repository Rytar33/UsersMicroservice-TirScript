﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class NewsTagConfiguration : IEntityTypeConfiguration<NewsTag>
{
    public void Configure(EntityTypeBuilder<NewsTag> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(50);

        builder.Property(p => p.DateCreated);

        builder.HasMany(p => p.News)
            .WithOne(p => p.NewsTag)
            .HasForeignKey(p => p.NewsTagId);
    }
}