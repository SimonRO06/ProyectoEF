using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class RepuestoRepository : IRepuestoRepository
{
    private readonly AppDbContext _context;

    public RepuestoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Repuesto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Repuestos
            .Include(p => p.DetallesOrdenes)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Repuesto?> GetByCodeAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Repuestos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Codigo == codigo, ct);
    }

    public async Task<IReadOnlyList<Repuesto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Repuestos
            .AsNoTracking()
            .OrderBy(p => p.Codigo)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Repuesto>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default)
    {
        var query = _context.Repuestos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Codigo!.Contains(search) ||
                p.Descripcion!.Contains(search));
        }

        return await query
            .OrderBy(p => p.Codigo)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = _context.Repuestos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Codigo!.Contains(search) ||
                p.Descripcion!.Contains(search));
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Repuesto repuesto, CancellationToken ct = default)
    {
        await _context.Repuestos.AddAsync(repuesto, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Repuesto repuesto, CancellationToken ct = default)
    {
        _context.Repuestos.Update(repuesto);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Repuesto repuesto, CancellationToken ct = default)
    {
        _context.Repuestos.Remove(repuesto);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByCodeAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Repuestos
            .AnyAsync(p => p.Codigo == codigo, ct);
    }

    public async Task<bool> IsInStockAsync(Guid sparePartId, int cantidadRequerida, CancellationToken ct = default)
    {
        var part = await _context.Repuestos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == sparePartId, ct);

        return part != null && part.CantidadStock >= cantidadRequerida;
    }

    public async Task UpdateStockAsync(Guid sparePartId, int quantityChange, CancellationToken ct = default)
    {
        var part = await _context.Repuestos.FirstOrDefaultAsync(p => p.Id == sparePartId, ct);
        if (part == null) return;

        part.GetType()
            .GetProperty("CantidadStock")!
            .SetValue(part, part.CantidadStock + quantityChange);

        _context.Repuestos.Update(part);
        await _context.SaveChangesAsync(ct);
    }
}