using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Repuestos;
public sealed class CreateOrdenServicioHandler(IRepuestoRepository repo) : IRequestHandler<CreateRepuesto, Guid>
{
    public async Task<Guid> Handle(CreateRepuesto req, CancellationToken ct)
    {
        var repuesto = new Repuesto(req.Codigo, req.Descripcion, req.CantidadStock, req.PrecioUnitario);

        await repo.AddAsync(repuesto, ct);
        return repuesto.Id;
    }
}