using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Citas;
public sealed class CreateCitaHandler(ICitaRepository repo) : IRequestHandler<CreateCita, Guid>
{
    public async Task<Guid> Handle(CreateCita req, CancellationToken ct)
    {
        var cita = new Cita(req.Fecha, req.Hora, req.Observaciones, req.ClienteId, req.VehiculoId);
        await repo.AddAsync(cita, ct);
        return cita.Id;
    }
}