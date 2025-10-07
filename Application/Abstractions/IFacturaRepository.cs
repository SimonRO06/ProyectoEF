using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IFacturaRepository
{
    Task<Factura?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Factura>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Factura>> GetByOrdenServicioIdAsync(Guid OrdenServicioId, CancellationToken ct = default);
    Task<IReadOnlyList<Factura>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Factura factura, CancellationToken ct = default);
    Task UpdateAsync(Factura factura, CancellationToken ct = default);
    Task RemoveAsync(Factura factura, CancellationToken ct = default);
}