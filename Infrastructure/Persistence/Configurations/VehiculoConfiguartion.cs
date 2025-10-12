using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class VehiculoConfiguartion : IEntityTypeConfiguration<Vehiculo>
{
       public void Configure(EntityTypeBuilder<Vehiculo> builder)
       {
              builder.ToTable("vehicles");
              builder.HasKey(v => v.Id);
              builder.Property(v => v.Año)
                     .IsRequired();

              builder.Property(v => v.NumeroSerie)
                     .IsRequired()
                     .HasColumnType("varchar(120)");

              builder.Property(v => v.Kilometraje)
                     .IsRequired();

              builder.HasOne(v => v.Cliente)
                     .WithMany(c => c.Vehiculos)
                     .HasForeignKey(v => v.ClienteId)
                     .OnDelete(DeleteBehavior.SetNull);

              builder.HasOne(v => v.Modelo)
                     .WithMany(m => m.Vehiculos)
                     .HasForeignKey(v => v.ModeloId)
                     .OnDelete(DeleteBehavior.SetNull);

              builder.HasMany(o => o.OrdenesServicios)
                     .WithOne(v => v.Vehiculo)
                     .HasForeignKey(o => o.VehiculoId)
                     .OnDelete(DeleteBehavior.SetNull);

              builder.HasMany(c => c.Citas)
                     .WithOne(v => v.Vehiculo)
                     .HasForeignKey(c => c.VehiculoId)
                     .OnDelete(DeleteBehavior.SetNull);

              builder.HasIndex(v => v.NumeroSerie)
                   .IsUnique();

    }
}
