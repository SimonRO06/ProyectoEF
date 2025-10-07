using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FechaEmision)
       .IsRequired()
       .HasColumnType("datetime2");

        builder.Property(p => p.Total)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.OrdenServicio)
            .WithMany(f => f.Facturas)
            .HasForeignKey(p => p.OrdenServicioId)
            .OnDelete(DeleteBehavior.SetNull); 
            
        builder.HasMany(r => r.Pagos)
        .WithOne(p => p.Factura)
        .HasForeignKey(r => r.FacturaId)
        .OnDelete(DeleteBehavior.SetNull); 

    }
}
