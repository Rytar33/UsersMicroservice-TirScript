using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Amount)
            .HasDefaultValue(0)
            .HasPrecision(18, 4);

        builder.Property(p => p.DateCreated);

        builder.HasOne(p => p.ProductCategory)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.CategoryId);

        builder.HasMany(p => p.ProductCategoryParameterValueProduct)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId);
    }
}