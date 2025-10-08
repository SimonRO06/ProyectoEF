using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<DetalleOrden> DetallesOrdenes => Set<DetalleOrden>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<OrdenServicio> OrdenesServicios => Set<OrdenServicio>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<Modelo> Modelos => Set<Modelo>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Rols => Set<Rol>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}