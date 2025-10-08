using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Entities.Auth;

namespace Application.Abstractions.Auth;
public interface IUsuarioService
{
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);
    IEnumerable<Usuario> Find(Expression<Func<Usuario, bool>> expression);
    Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct = default);
    Task<(int totalRegistros, IEnumerable<Usuario> registros)> GetPagedAsync(int pageIndex, int pageSize,string search);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Usuario entity, CancellationToken ct = default);
    Task UpdateAsync(Usuario entity, CancellationToken ct = default);
    Task RemoveAsync(Usuario entity, CancellationToken ct = default);
    Task<Usuario?> GetByUserNameAsync(string userName,CancellationToken ct = default);

    Task<Usuario> GetByRefreshTokenAsync(string refreshToken);
}
