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
               .HasColumnType("varchar(100)");

        builder.Property(v => v.Kilometraje)
               .IsRequired();

        builder.HasOne(c => c.Cliente)
               .WithMany(c => c.Vehiculos)
               .HasForeignKey(p => p.ClienteId)
               .IsRequired();

        builder.HasOne(p => p.Modelo)
               .WithMany(m => m.Vehiculos)
               .HasForeignKey(m => m.ModeloId)
               .IsRequired();

        builder.HasMany(p => p.OrdenesServicios)
               .WithOne(os => os.Vehiculo)
               .HasForeignKey(os => os.VehiculoId)
               .IsRequired();

    }
}
