using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Vehiculos;
public sealed class CreateVehiculoHandler(IVehiculoRepository repo) : IRequestHandler<CreateVehiculo, Guid>
{
    public async Task<Guid> Handle(CreateVehiculo req, CancellationToken ct)
    {
        var vehiculo = new Vehiculo(req.Año, req.NumeroSerie, req.Kilometraje, req.ClienteId, req.ModeloId)
        {
            ClienteId = req.ClienteId,
            ModeloId = req.ModeloId
        };

        await repo.AddAsync(vehiculo, ct);
        return vehiculo.Id;
    }
}