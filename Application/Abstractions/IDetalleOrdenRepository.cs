using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IDetalleOrdenRepository
{
    Task<DetalleOrden?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<DetalleOrden>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<DetalleOrden>> GetByOrdenServicioIdAsync(Guid OrdenServicioId, CancellationToken ct = default);
    Task<IReadOnlyList<DetalleOrden>> GetByRepuestoIdAsync(Guid RepuestoId, CancellationToken ct = default);
    Task<IReadOnlyList<DetalleOrden>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(DetalleOrden detalleOrden, CancellationToken ct = default);
    Task UpdateAsync(DetalleOrden detalleOrden, CancellationToken ct = default);
    Task RemoveAsync(DetalleOrden detalleOrden, CancellationToken ct = default);
}