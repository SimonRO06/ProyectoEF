using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DetalleOrdenConfiguration : IEntityTypeConfiguration<DetalleOrden>
{
    public void Configure(EntityTypeBuilder<DetalleOrden> builder)
    {
        builder.ToTable("order_details");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Cantidad)
       .IsRequired();

        builder.Property(d => d.CostoUnitario)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

    
        builder.HasOne(d => d.OrdenServicio)
            .WithMany(de => de.DetallesOrdenes)
            .HasForeignKey(d => d.OrdenServicioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Repuesto)
            .WithMany(r => r.DetallesOrdenes)
            .HasForeignKey(d => d.RepuestoId)
            .OnDelete(DeleteBehavior.SetNull);


    }
}
 