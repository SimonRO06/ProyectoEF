using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.OrdenesServicios;
public sealed class CreateOrdenServicioHandler(IOrdenServicioRepository repo) : IRequestHandler<CreateOrdenServicio, Guid>
{
    public async Task<Guid> Handle(CreateOrdenServicio req, CancellationToken ct)
    {
        var ordenServicio = new OrdenServicio(req.TipoServicio, req.FechaIngreso, req.FechaEstimadaEntrega, req.Estado, req.UserMemberId, req.VehiculoId)
        {
            UserMemberId = req.UserMemberId,
            VehiculoId = req.VehiculoId
        };

        await repo.AddAsync(ordenServicio, ct);
        return ordenServicio.Id;
    }
}