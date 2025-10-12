using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class CitaRepository : ICitaRepository
{
    private readonly AppDbContext _context;

    public CitaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cita?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Citas
            .Include(c => c.Cliente)
            .Include(c => c.Vehiculo)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Cita>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Citas
            .Include(c => c.Cliente)
            .Include(c => c.Vehiculo)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Cita>> GetByClienteIdAsync(Guid clienteId, CancellationToken ct = default)
    {
        return await _context.Citas
            .Where(c => c.ClienteId == clienteId)
            .Include(c => c.Vehiculo)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Cita>> GetByVehiculoIdAsync(Guid vehiculoId, CancellationToken ct = default)
    {
        return await _context.Citas
            .Where(c => c.VehiculoId == vehiculoId)
            .Include(c => c.Cliente)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Cita>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default)
    {
        var query = _context.Citas
            .Include(c => c.Cliente)
            .Include(c => c.Vehiculo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.ToLower();
            query = query.Where(c =>
                c.Observaciones.ToLower().Contains(q) ||
                c.Cliente!.Nombre.ToLower().Contains(q));
        }

        return await query
            .OrderByDescending(c => c.Fecha)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? q, CancellationToken ct = default)
    {
        var query = _context.Citas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.ToLower();
            query = query.Where(c =>
                c.Observaciones.ToLower().Contains(q) ||
                c.Cliente!.Nombre.ToLower().Contains(q));
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Cita cita, CancellationToken ct = default)
    {
        await _context.Citas.AddAsync(cita, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Cita cita, CancellationToken ct = default)
    {
        _context.Citas.Update(cita);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Cita cita, CancellationToken ct = default)
    {
        _context.Citas.Remove(cita);
        await _context.SaveChangesAsync(ct);
    }
}