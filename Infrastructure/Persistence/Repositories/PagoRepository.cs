using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class PagoRepository : IPagoRepository
{
    private readonly AppDbContext _context;

    public PagoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pago?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Pagos
            .Include(p => p.Factura)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Pago>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Pagos
            .Include(p => p.Factura)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Pago>> GetByFacturaIdAsync(Guid facturaId, CancellationToken ct = default)
    {
        return await _context.Pagos
            .Where(p => p.FacturaId == facturaId)
            .Include(p => p.Factura)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? q, CancellationToken ct = default)
    {
        var query = _context.Pagos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(p =>
                p.MetodoPago.ToString().Contains(q));
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Pago pago, CancellationToken ct = default)
    {
        await _context.Pagos.AddAsync(pago, ct);
    }

    public Task UpdateAsync(Pago pago, CancellationToken ct = default)
    {
        _context.Pagos.Update(pago);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Pago pago, CancellationToken ct = default)
    {
        _context.Pagos.Remove(pago);
        return Task.CompletedTask;
    }
}