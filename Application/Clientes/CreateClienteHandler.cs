using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Clientes;
public sealed class CreateClienteHandler(IClienteRepository repo) : IRequestHandler<CreateCliente, Guid>
{
    public async Task<Guid> Handle(CreateCliente req, CancellationToken ct)
    {
        var cliente = new Cliente(req.Nombre, req.Telefono, req.Correo);
        await repo.AddAsync(cliente, ct);
        return cliente.Id;
    }
}