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
               .HasColumnType("varchar(50)");

        builder.Property(c => c.Correo)
            .IsRequired()
            .HasColumnType("varchar(100)");

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
    }
}
