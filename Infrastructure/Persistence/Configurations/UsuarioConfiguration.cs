using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario> 
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Nombre)
               .IsRequired()
               .HasColumnType("varchar(120)");

        builder.Property(u => u.Correo)
            .IsRequired()
            .HasColumnType("varchar(150)");

        builder.Property(u => u.ContraseñaHasheada)
               .HasColumnName("password_hash")
               .HasColumnType("varchar(200)")
               .IsRequired(false);

        builder.Property(u => u.Rol)
               .HasConversion<string>()
               .HasColumnName("rol")
               .HasColumnType("varchar(20)")
               .IsRequired();

        builder.HasMany(o => o.OrdenesServicios)
        .WithOne(u => u.Usuario)
        .HasForeignKey(o => o.UsuarioId)
        .OnDelete(DeleteBehavior.SetNull); 

        builder.HasIndex(u => u.Correo)
                   .IsUnique();
    }
}
