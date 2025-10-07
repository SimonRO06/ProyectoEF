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
        builder.HasKey(o => o.Id);

        builder.Property(p => p.Cantidad)
       .IsRequired();

        builder.Property(p => p.CostoUnitario)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

    
        builder.HasOne(os => os.OrdenServicio)
            .WithMany(o => o.DetallesOrdenes)
            .HasForeignKey(os => os.OrdenServicioId)
            .IsRequired();

        builder.HasOne(r => r.Repuesto)
            .WithMany(o => o.DetallesOrden)
            .HasForeignKey(r => r.RepuestoId)
            .IsRequired();


    }
}
 