using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IVehiculoRepository
{
    Task<Vehiculo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Vehiculo>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Vehiculo>> GetByClienteIdAsync(Guid ClienteId, CancellationToken ct = default);
    Task<IReadOnlyList<Vehiculo>> GetByModeloIdAsync(Guid ModeloId, CancellationToken ct = default);
    Task<IReadOnlyList<Vehiculo>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Vehiculo vehiculo, CancellationToken ct = default);
    Task UpdateAsync(Vehiculo vehiculo, CancellationToken ct = default);
    Task RemoveAsync(Vehiculo vehiculo, CancellationToken ct = default);
}