using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class DetalleOrdenRepository : IDetalleOrdenRepository
{
    private readonly AppDbContext _context;

    public DetalleOrdenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DetalleOrden?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.DetallesOrdenes
            .Include(od => od.OrdenServicio)
            .Include(od => od.Repuesto)
            .FirstOrDefaultAsync(od => od.Id == id, ct);
    }

    public async Task<IReadOnlyList<DetalleOrden>> GetByOrdenServicioIdAsync(Guid ordenServicioId, CancellationToken ct = default)
    {
        return await _context.DetallesOrdenes
            .Include(od => od.Repuesto)
            .Where(od => od.OrdenServicioId == ordenServicioId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<DetalleOrden>> GetByRepuestoIdAsync(Guid repuestoId, CancellationToken ct = default)
    {
        return await _context.DetallesOrdenes
            .Include(od => od.OrdenServicio)
            .Where(od => od.RepuestoId == repuestoId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<DetalleOrden>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.DetallesOrdenes
            .Include(od => od.OrdenServicio)
            .Include(od => od.Repuesto)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(DetalleOrden detalleOrden, CancellationToken ct = default)
    {
        await _context.DetallesOrdenes.AddAsync(detalleOrden, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DetalleOrden detalleOrden, CancellationToken ct = default)
    {
        _context.DetallesOrdenes.Update(detalleOrden);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(DetalleOrden detalleOrden, CancellationToken ct = default)
    {
        _context.DetallesOrdenes.Remove(detalleOrden);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid ordenServicioId, Guid repuestoId, CancellationToken ct = default)
    {
        return await _context.DetallesOrdenes
            .AnyAsync(od => od.OrdenServicioId == ordenServicioId && od.RepuestoId == repuestoId, ct);
    }
}