using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Facturas;
public sealed class CreateFacturaHandler(IFacturaRepository repo) : IRequestHandler<CreateFactura, Guid>
{
    public async Task<Guid> Handle(CreateFactura req, CancellationToken ct)
    {
        var factura = new Factura(req.FechaEmision, req.Impuestos, req.Total, req.OrdenServicioId)
        {
            OrdenServicioId = req.OrdenServicioId
        };

        await repo.AddAsync(factura, ct);
        return factura.Id;
    }
}