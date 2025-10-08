using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Clientes
            .Include(c => c.Vehiculos)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Cliente?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Correo!.ToLower() == email.ToLower(), ct);
    }

    public async Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Clientes
            .Include(c => c.Vehiculos)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Cliente>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default)
    {
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.Nombre!.Contains(search) ||
                c.Correo!.Contains(search) ||
                c.Telefono!.Contains(search));
        }

        return await query
            .OrderBy(c => c.Nombre)
            .Skip((page - 1) * size)
            .Take(size)
            .Include(c => c.Vehiculos)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.Nombre!.Contains(search) ||
                c.Correo!.Contains(search) ||
                c.Telefono!.Contains(search));
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Clientes
            .AnyAsync(c => c.Correo!.ToLower() == email.ToLower(), ct);
    }

    public async Task AddAsync(Cliente cliente, CancellationToken ct = default)
    {
        await _context.Clientes.AddAsync(cliente, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Cliente cliente, CancellationToken ct = default)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Cliente cliente, CancellationToken ct = default)
    {
        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync(ct);
    }
}