using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RepuestoConfiguration : IEntityTypeConfiguration<Repuesto>
{
       public void Configure(EntityTypeBuilder<Repuesto> builder)
       {
              
              builder.ToTable("spare_parts");
              builder.HasKey(c => c.Id);
              builder.Property(p => p.Codigo)
                     .IsRequired()
                     .HasColumnType("varchar(30)");

              builder.Property(p => p.Descripcion)
                     .IsRequired()
                     .HasColumnType("varchar(100)");

              builder.Property(p => p.CantidadStock)
                     .IsRequired();

              builder.Property(p => p.PrecioUnitario)
                     .IsRequired()
                     .HasColumnType("decimal(18,2)");
                     
              builder.HasMany(r => r.DetallesOrden)
                     .WithOne(d => d.Repuesto)
                     .HasForeignKey(d => d.RepuestoId)
                     .IsRequired();

    }
}
