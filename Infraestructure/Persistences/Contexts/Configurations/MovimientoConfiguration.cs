using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistences.Contexts.Configurations
{
    public class MovimientoConfiguration : IEntityTypeConfiguration<Movimiento>
    {
        public void Configure(EntityTypeBuilder<Movimiento> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__Movimien__3213E83FB9A9236E");

            builder.ToTable("Movimiento");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.CreateAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("createAt");
            builder.Property(e => e.ProductId).HasColumnName("productId");
            builder.Property(e => e.Quantity).HasColumnName("quantity");
            builder.Property(e => e.TypeMove)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("typeMove");

            builder.HasOne(d => d.Product).WithMany(p => p.Movimientos)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Movimient__produ__5535A963");
        }
    }
}
