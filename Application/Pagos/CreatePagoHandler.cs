using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Pagos;
public sealed class CreateOrdenServicioHandler(IPagoRepository repo) : IRequestHandler<CreatePago, Guid>
{
    public async Task<Guid> Handle(CreatePago req, CancellationToken ct)
    {
        var pago = new Pago(req.FechaPago, req.Monto, req.MetodoPago, req.FacturaId)
        {
            FacturaId = req.FacturaId
        };

        await repo.AddAsync(pago, ct);
        return pago.Id;
    }
}