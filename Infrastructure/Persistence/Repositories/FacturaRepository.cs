using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class FacturaRepository : IFacturaRepository
{
    private readonly AppDbContext _context;

    public FacturaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Factura?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Facturas
            .Include(i => i.OrdenServicio)
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public async Task<IReadOnlyList<Factura>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Facturas
            .Include(i => i.OrdenServicio)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Factura?> GetByOrdenServicioIdAsync(Guid ordenServicioId, CancellationToken ct = default)
    {
        return await _context.Facturas
            .Include(i => i.OrdenServicio)
            .FirstOrDefaultAsync(i => i.OrdenServicioId == ordenServicioId, ct);
    }

    public async Task<IReadOnlyList<Factura>> GetByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default)
    {
        return await _context.Facturas
            .Include(i => i.OrdenServicio)
            .Where(i => i.FechaEmision >= fechaInicio && i.FechaEmision <= fechaFin)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(Factura factura, CancellationToken ct = default)
    {
        await _context.Facturas.AddAsync(factura, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Factura factura, CancellationToken ct = default)
    {
        _context.Facturas.Update(factura);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Factura factura, CancellationToken ct = default)
    {
        _context.Facturas.Remove(factura);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<decimal> GetIngresosTotalesAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        return await _context.Facturas
            .Where(i => i.FechaEmision >= startDate && i.FechaEmision <= endDate)
            .SumAsync(i => i.Total, ct);
    }
}