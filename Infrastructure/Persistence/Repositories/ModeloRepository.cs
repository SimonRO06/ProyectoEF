using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class ModeloRepository : IModeloRepository
{
    private readonly AppDbContext _context;

    public ModeloRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Modelo>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Modelos
            .AsNoTracking()
            .Include(m => m.Marca)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Modelo>> GetByMarcaIdAsync(Guid marcaId, CancellationToken ct = default)
    {
        return await _context.Modelos
            .AsNoTracking()
            .Where(m => m.MarcaId == marcaId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Modelo>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default)
    {
        var query = _context.Modelos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(m => m.Nombre.Contains(q));

        return await query
            .AsNoTracking()
            .Include(m => m.Marca)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? q, CancellationToken ct = default)
    {
        var query = _context.Modelos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(m => m.Nombre.Contains(q));

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Modelo modelo, CancellationToken ct = default)
    {
        await _context.Modelos.AddAsync(modelo, ct);
    }

    public Task UpdateAsync(Modelo modelo, CancellationToken ct = default)
    {
        _context.Modelos.Update(modelo);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Modelo modelo, CancellationToken ct = default)
    {
        _context.Modelos.Remove(modelo);
        return Task.CompletedTask;
    }
}