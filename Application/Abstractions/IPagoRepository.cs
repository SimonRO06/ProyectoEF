using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;

public interface IPagoRepository
{
    Task<Pago?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Pago>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Pago>> GetByFacturaIdAsync(Guid FacturaId, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Pago pago, CancellationToken ct = default);
    Task UpdateAsync(Pago pago, CancellationToken ct = default);
    Task RemoveAsync(Pago pago, CancellationToken ct = default);
}