using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Marcas;
public sealed class CreateFacturaHandler(IMarcaRepository repo) : IRequestHandler<CreateMarca, Guid>
{
    public async Task<Guid> Handle(CreateMarca req, CancellationToken ct)
    {
        var marca = new Marca(req.Nombre);
        await repo.AddAsync(marca, ct);
        return marca.Id;
    }
}