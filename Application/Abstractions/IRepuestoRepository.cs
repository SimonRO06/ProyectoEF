using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IRepuestoRepository
{
    Task<Repuesto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Repuesto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Repuesto>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Repuesto repuesto, CancellationToken ct = default);
    Task UpdateAsync(Repuesto repuesto, CancellationToken ct = default);
    Task RemoveAsync(Repuesto repuesto, CancellationToken ct = default);
}