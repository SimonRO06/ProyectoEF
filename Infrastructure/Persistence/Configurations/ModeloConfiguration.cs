using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ModeloConfiguration : IEntityTypeConfiguration<Modelo>
{
    public void Configure(EntityTypeBuilder<Modelo> builder)
    {
        builder.ToTable("models");
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Nombre)
            .IsRequired()
            .HasColumnType("varchar(120)");

        builder.HasOne(m => m.Marca)
            .WithMany(ma => ma.Modelos)
            .HasForeignKey(m => m.MarcaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(v => v.Vehiculos)
            .WithOne(m => m.Modelo)
            .HasForeignKey(v => v.ModeloId)
            .OnDelete(DeleteBehavior.SetNull); 


    }
}
