using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;
public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Monto)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.FechaPago)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(p => p.MetodoPago)
            .IsRequired()
            .HasConversion<int>(); 


        builder.HasOne(p => p.Factura)
            .WithMany(f => f.Pagos) 
            .HasForeignKey(p => p.FacturaId)
            .IsRequired();

    }
}
