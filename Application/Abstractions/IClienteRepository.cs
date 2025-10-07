using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(Guid Id, CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> GetPagedAsync(int page,int size,string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Cliente cliente, CancellationToken ct = default);
    Task UpdateAsync(Cliente cliente, CancellationToken ct = default);
    Task RemoveAsync(Cliente cliente, CancellationToken ct = default);
}