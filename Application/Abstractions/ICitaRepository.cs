using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface ICitaRepository
{
    Task<Cita?> GetByIdAsync(Guid Id, CancellationToken ct = default);
    Task<IReadOnlyList<Cita>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Cita>> GetByClienteIdAsync(Guid clienteId, CancellationToken ct = default);
    Task<IReadOnlyList<Cita>> GetByVehiculoIdAsync(Guid vehiculoId, CancellationToken ct = default);
    Task<IReadOnlyList<Cita>> GetPagedAsync(int page,int size,string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Cita cita, CancellationToken ct = default);
    Task UpdateAsync(Cita cita, CancellationToken ct = default);
    Task RemoveAsync(Cita cita, CancellationToken ct = default);
}