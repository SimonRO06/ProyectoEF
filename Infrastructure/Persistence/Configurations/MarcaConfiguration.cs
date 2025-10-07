using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MarcaConfiguration : IEntityTypeConfiguration<Marca>
{
    public void Configure(EntityTypeBuilder<Marca> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .IsRequired();
        builder.Property(m => m.Nombre)
            .IsRequired()
            .HasColumnType("varchar(100)");
        builder.HasMany(m => m.Modelos)
            .WithOne(mod => mod.Marca)
            .HasForeignKey(mod => mod.MarcaId)
            .IsRequired();

    }
}
