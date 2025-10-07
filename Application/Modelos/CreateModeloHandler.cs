using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Modelos;
public sealed class CreateModeloHandler(IModeloRepository repo) : IRequestHandler<CreateModelo, Guid>
{
    public async Task<Guid> Handle(CreateModelo req, CancellationToken ct)
    {
        var modelo = new Modelo(req.Nombre, req.MarcaId)
        {
            MarcaId = req.MarcaId
        };

        await repo.AddAsync(modelo, ct);
        return modelo.Id;
    }
}