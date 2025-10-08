using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Auth;
public class UserMemberConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("users_members");

        builder.HasKey(u => u.Id);
        builder.Property(di => di.Id)
               .ValueGeneratedOnAdd()
               .IsRequired()
               .HasColumnName("id");

        builder.Property(u => u.Nombre)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("name");

        builder.Property(u => u.Nombre)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("email");

        builder.HasIndex(u => u.Correo).IsUnique();

        builder.Property(u => u.Contraseña)
           .HasColumnType("varchar")
           .HasMaxLength(255)
           .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("createdAt")
            .HasColumnType("date")
            .HasDefaultValueSql("CURRENT_DATE")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updatedAt")
            .HasColumnType("date")
            .HasDefaultValueSql("CURRENT_DATE")
            .ValueGeneratedOnAddOrUpdate();
        builder
               .HasMany(p => p.Rols)
               .WithMany(r => r.Usuarios)
               .UsingEntity<UserMemberRol>(

                   j => j
                   .HasOne(pt => pt.Rol)
                   .WithMany(t => t.UserMemberRols)
                   .HasForeignKey(ut => ut.RolId),

                   j => j
                   .HasOne(et => et.Usuarios)
                   .WithMany(et => et.UserMemberRols)
                   .HasForeignKey(el => el.UsuarioId),

                   j =>
                   {
                       j.ToTable("users_rols");
                       j.HasKey(t => new { t.UsuarioId, t.RolId });

                   });

                builder.HasMany(p => p.RefreshTokens)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId);
    }
}