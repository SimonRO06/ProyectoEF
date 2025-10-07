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
        builder.HasKey(c => c.Id);
        builder.Property(p => p.FechaEmision)
       .IsRequired()
       .HasColumnType("datetime2");

        builder.Property(p => p.Impuestos)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Total)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.OrdenServicio)
            .WithMany() 
            .HasForeignKey(p => p.OrdenServicioId)
            .IsRequired();

    }
}
