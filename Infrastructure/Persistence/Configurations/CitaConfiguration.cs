using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Auth;
public class CitaConfiguration : IEntityTypeConfiguration<Cita>
{
    public void Configure(EntityTypeBuilder<Cita> builder)
    {
        builder.ToTable("meetings");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Fecha)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(c => c.Hora)
            .IsRequired()
            .HasColumnType("time");

        builder.Property(c => c.Observaciones)
            .IsRequired()
            .HasColumnType("text");;

        builder.HasOne(c => c.Cliente)
            .WithMany()
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Vehiculo)
            .WithMany()
            .HasForeignKey(c => c.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}