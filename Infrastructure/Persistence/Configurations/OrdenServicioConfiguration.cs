using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
{
    public void Configure(EntityTypeBuilder<OrdenServicio> builder)
    {
        builder.ToTable("service_orders");
        builder.HasKey(os => os.Id);
        builder.Property(os => os.TipoServicio)
            .IsRequired()
            .HasConversion<int>(); 

        builder.Property(os => os.FechaIngreso)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(os => os.FechaEstimadaEntrega)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(os => os.Estado)
            .IsRequired()
            .HasConversion<int>(); 


        builder.HasOne(os => os.Usuario)
            .WithMany(u => u.OrdenesServicios) 
            .HasForeignKey(os => os.UsuarioId)
            .IsRequired(false);

        builder.HasOne(os => os.Vehiculo)
            .WithMany(v => v.OrdenesServicios) 
            .HasForeignKey(os => os.VehiculoId)
            .IsRequired(false);

        builder.HasMany(d => d.DetallesOrdenes)
            .WithOne(os => os.OrdenServicio)  
            .HasForeignKey(d => d.OrdenServicioId)
            .IsRequired();

        builder.HasMany(f => f.Facturas)
            .WithOne(os => os.OrdenServicio)
            .HasForeignKey(f => f.OrdenServicioId)
            .IsRequired();

    }
}
