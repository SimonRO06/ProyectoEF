using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class MarcaRepository : IMarcaRepository
{
    private readonly AppDbContext _context;

    public MarcaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Marca?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Marcas
            .FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    public async Task<IReadOnlyList<Marca>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Marcas
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Marca>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default)
    {
        var query = _context.Marcas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(m => m.Nombre.Contains(q));

        return await query
            .AsNoTracking()
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? q, CancellationToken ct = default)
    {
        var query = _context.Marcas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(m => m.Nombre.Contains(q));

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Marca marca, CancellationToken ct = default)
    {
        await _context.Marcas.AddAsync(marca, ct);
    }

    public Task UpdateAsync(Marca marca, CancellationToken ct = default)
    {
        _context.Marcas.Update(marca);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Marca marca, CancellationToken ct = default)
    {
        _context.Marcas.Remove(marca);
        return Task.CompletedTask;
    }
}