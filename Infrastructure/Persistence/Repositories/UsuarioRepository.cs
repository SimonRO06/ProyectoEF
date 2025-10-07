using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Vehiculo?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .Include(c => c.Region)
            .ThenInclude(r => r.Vehiculo)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Vehiculo>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .Include(c => c.Vehiculo)
            .ThenInclude(r => r.Vehiculo)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Vehiculo>> GetByRegionIdAsync(Guid regionId, CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .Where(c => c.RegionId == regionId)
            .Include(c => c.Region)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Vehiculo>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default)
    {
        var query = _context.Vehiculos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(c => c.Name!.Contains(q));
        }

        return await query
            .Include(c => c.Region)
            .OrderBy(c => c.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? q, CancellationToken ct = default)
    {
        var query = _context.Vehiculos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(c => c.Name!.Contains(q));
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Vehiculo Vehiculo, CancellationToken ct = default)
    {
        await _context.Vehiculos.AddAsync(Vehiculo, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Vehiculo Vehiculo, CancellationToken ct = default)
    {
        _context.Vehiculos.Update(Vehiculo);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Vehiculo Vehiculo, CancellationToken ct = default)
    {
        _context.Vehiculos.Remove(Vehiculo);
        await _context.SaveChangesAsync(ct);
    }
}