using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistences.Contexts.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__Products__3213E83F0759DEFF");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Categorie)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("categorie");
            builder.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            builder.Property(e => e.Name)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("name");
            builder.Property(e => e.Price).HasColumnName("price");
            builder.Property(e => e.Stock).HasColumnName("stock");
        }
    }
}
