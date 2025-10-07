using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nombre)
               .IsRequired()
               .HasColumnType("varchar(50)");

        builder.Property(c => c.Telefono)
                .IsRequired()
                .HasColumnType("varchar(30)");

        builder.Property(c => c.Correo)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.HasMany(v => v.Vehiculos)
                .WithOne(c => c.Cliente)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.SetNull); 
        
   }
}
