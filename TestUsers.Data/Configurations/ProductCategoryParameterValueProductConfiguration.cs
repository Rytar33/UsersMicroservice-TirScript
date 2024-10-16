﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class ProductCategoryParameterValueProductConfiguration : IEntityTypeConfiguration<ProductCategoryParameterValueProduct>
{
    public void Configure(EntityTypeBuilder<ProductCategoryParameterValueProduct> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DateCreated);

        builder.HasOne(p => p.ProductCategoryParameterValue)
            .WithMany(p => p.ProductCategoryParameterValueProduct)
            .HasForeignKey(p => p.ProductCategoryParameterValueId);

        builder.HasOne(p => p.Product)
            .WithMany(p => p.ProductCategoryParameterValueProduct)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}