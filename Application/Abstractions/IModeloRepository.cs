using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IModeloRepository
{
    Task<Modelo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Modelo>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Modelo>> GetByMarcaIdAsync(Guid MarcaId, CancellationToken ct = default);
    Task<IReadOnlyList<Modelo>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Modelo modelo, CancellationToken ct = default);
    Task UpdateAsync(Modelo modelo, CancellationToken ct = default);
    Task RemoveAsync(Modelo modelo, CancellationToken ct = default);
}
