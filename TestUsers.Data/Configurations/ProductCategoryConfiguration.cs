using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(100);

        builder.HasMany(p => p.ChildCategories)
            .WithOne(p => p.ParentCategory)
            .HasForeignKey(p => p.ParentCategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.Products)
            .WithOne(p => p.ProductCategory)
            .HasForeignKey(p => p.CategoryId);

        builder.HasMany(p => p.Parameters)
            .WithOne(p => p.ProductCategory)
            .HasForeignKey(p => p.ProductCategoryId);

        builder.HasMany(p => p.UserSaveFilters)
            .WithOne(p => p.ProductCategory)
            .HasForeignKey(p => p.CategoryId);
    }
}