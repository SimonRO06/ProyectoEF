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
    Task<Factura?> GetByOrdenServicioIdAsync(Guid ordenServicioId, CancellationToken ct = default);
    Task<IReadOnlyList<Factura>> GetByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken ct = default);
    Task AddAsync(Factura factura, CancellationToken ct = default);
    Task UpdateAsync(Factura factura, CancellationToken ct = default);
    Task RemoveAsync(Factura factura, CancellationToken ct = default);
    Task<decimal> GetIngresosTotalesAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}