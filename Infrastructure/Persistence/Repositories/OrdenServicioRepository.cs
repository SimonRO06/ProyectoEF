using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class OrdenServicioRepository : IOrdenServicioRepository
{
    private readonly AppDbContext _context;

    public OrdenServicioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrdenServicio?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.OrdenesServicios
            .Include(o => o.Vehiculo)
                .ThenInclude(v => v.Cliente)
            .Include(o => o.DetallesOrdenes)
                .ThenInclude(d => d.Repuesto)
            .Include(o => o.Facturas)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IReadOnlyList<OrdenServicio>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.OrdenesServicios
            .Include(o => o.Vehiculo)
            .Include(o => o.Facturas)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<OrdenServicio>> GetByUserMemberIdAsync(int userMemberId, CancellationToken ct = default)
    {
        return await _context.OrdenesServicios
            .Where(o => o.UserMemberId == userMemberId)
            .Include(o => o.Vehiculo)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<OrdenServicio>> GetByVehiculoIdAsync(Guid vehiculoId, CancellationToken ct = default)
    {
        return await _context.OrdenesServicios
            .Where(o => o.VehiculoId == vehiculoId)
            .Include(o => o.Facturas)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(OrdenServicio ordenServicio, CancellationToken ct = default)
    {
        await _context.OrdenesServicios.AddAsync(ordenServicio, ct);
    }

    public Task UpdateAsync(OrdenServicio ordenServicio, CancellationToken ct = default)
    {
        _context.OrdenesServicios.Update(ordenServicio);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(OrdenServicio ordenServicio, CancellationToken ct = default)
    {
        _context.OrdenesServicios.Remove(ordenServicio);
        return Task.CompletedTask;
    }
}