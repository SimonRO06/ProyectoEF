using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class VehiculoRepository : IVehiculoRepository
{
    private readonly AppDbContext _context;

    public VehiculoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Vehiculo?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .Include(v => v.Cliente)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<Vehiculo?> GetByVinAsync(string numeroSerie, CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .Include(v => v.Cliente)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.NumeroSerie == numeroSerie, ct);
    }

    public async Task<IReadOnlyList<Vehiculo>> GetByCustomerIdAsync(Guid clienteId, CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .Where(v => v.ClienteId == clienteId)
            .Include(v => v.Cliente)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Vehiculo>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .Include(v => v.Cliente)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Vehiculo>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default)
    {
        var query = _context.Vehiculos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(v =>
                v.Modelo.Nombre!.Contains(search) ||
                v.NumeroSerie!.Contains(search));
        }

        return await query
            .Include(v => v.Cliente)
            .OrderBy(v => v.Modelo)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = _context.Vehiculos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(v =>
                v.Modelo.Nombre!.Contains(search) ||
                v.NumeroSerie!.Contains(search));
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsVinAsync(string vin, CancellationToken ct = default)
    {
        return await _context.Vehiculos.AnyAsync(v => v.NumeroSerie == vin, ct);
    }

    public async Task AddAsync(Vehiculo vehiculo, CancellationToken ct = default)
    {
        await _context.Vehiculos.AddAsync(vehiculo, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Vehiculo vehiculo, CancellationToken ct = default)
    {
        _context.Vehiculos.Update(vehiculo);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Vehiculo vehiculo, CancellationToken ct = default)
    {
        _context.Vehiculos.Remove(vehiculo);
        await _context.SaveChangesAsync(ct);
    }
}