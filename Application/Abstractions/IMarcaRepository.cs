using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IMarcaRepository
{
    Task<Marca?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Marca>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Marca>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Marca marca, CancellationToken ct = default);
    Task UpdateAsync(Marca marca, CancellationToken ct = default);
    Task RemoveAsync(Marca marca, CancellationToken ct = default);
}