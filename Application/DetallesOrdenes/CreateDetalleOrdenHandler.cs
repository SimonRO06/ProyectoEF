using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.DetallesOrdenes;
public sealed class CreateDetalleOrdenHandler(IDetalleOrdenRepository repo) : IRequestHandler<CreateDetalleOrden, Guid>
{
    public async Task<Guid> Handle(CreateDetalleOrden req, CancellationToken ct)
    {
        var detalleOrden = new DetalleOrden(req.Cantidad, req.CostoUnitario, req.OrdenServicioId, req.RepuestoId)
        {
            OrdenServicioId = req.OrdenServicioId,
            RepuestoId = req.RepuestoId
        };

        await repo.AddAsync(detalleOrden, ct);
        return detalleOrden.Id;
    }
}