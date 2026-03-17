using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistences.Contexts.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK__User__3213E83F1940B930");

            builder.ToTable("User");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            builder.Property(e => e.Fullname)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("fullname");
            builder.Property(e => e.Password)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("password");
            builder.Property(e => e.RolId).HasColumnName("rolId");
            builder.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            builder.Property(e => e.Username)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("username");

            builder.HasOne(d => d.Rol).WithMany(p => p.Users)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__rolId__4E88ABD4");
        }
    }
}
