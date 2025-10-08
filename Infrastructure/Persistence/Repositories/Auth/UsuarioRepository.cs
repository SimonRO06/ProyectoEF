using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.Abstractions.Auth;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Auth;
public class UserMemberRepository(AppDbContext db) : IUsuarioService
{

    public Task<int> CountAsync(string? search = null, CancellationToken ct = default)
    {
        var query = db.Usuarios.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(p => EF.Functions.ILike(p.Nombre, term));
        }
        return query.CountAsync(ct);
    }
    public async Task<Usuario?> GetByUserNameAsync(string nombre, CancellationToken ct = default)
    {
        return await db.Usuarios
                .Include(u => u.UserMemberRols)
                .Include(u => u.Rols)
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Nombre, nombre));

    }
    public virtual IEnumerable<Usuario> Find(Expression<Func<Usuario, bool>> expression)
    {
        return db.Set<Usuario>().Where(expression);
    }

    public Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
        => db.Usuarios.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Usuario> GetByRefreshTokenAsync(string refreshToken)
    {
        var user = await db.Usuarios
                    .Include(u => u.Rols)
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
        if (user == null)
            throw new InvalidOperationException("UserMember not found for the given refresh token.");
        return user;
    }


    public async Task AddAsync(Usuario usuario, CancellationToken ct = default)
    {
        db.Usuarios.Add(usuario);
        // await db.SaveChangesAsync(ct);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
    {
        db.Usuarios.Update(usuario);
        // await db.SaveChangesAsync(ct);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Usuario usuario, CancellationToken ct = default)
    {
        db.Usuarios.Remove(usuario);
        await db.SaveChangesAsync(ct);
    }

    async Task<IEnumerable<Usuario>> IUsuarioService.GetAllAsync(CancellationToken ct)
    {
        return await db.Usuarios.AsNoTracking().ToListAsync(ct);
    }

    public async Task<(int totalRegistros, IEnumerable<Usuario> registros)> GetPagedAsync(int pageIndex, int pageSize, string search)
    {
        var totalRegistros = await db.Set<Usuario>()
                            .CountAsync();

        var registros = await db.Set<Usuario>()
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return (totalRegistros, registros);
    }
}