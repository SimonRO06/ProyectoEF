using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IOrdenServicioRepository
{
    Task<OrdenServicio?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<OrdenServicio>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrdenServicio>> GetByUsuarioIdAsync(Guid UsuarioId, CancellationToken ct = default);
    Task<IReadOnlyList<OrdenServicio>> GetByVehiculoIdAsync(Guid VehiculoId, CancellationToken ct = default);
    Task<IReadOnlyList<OrdenServicio>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(OrdenServicio ordenServicio, CancellationToken ct = default);
    Task UpdateAsync(OrdenServicio ordenServicio, CancellationToken ct = default);
    Task RemoveAsync(OrdenServicio ordenServicio, CancellationToken ct = default);
}