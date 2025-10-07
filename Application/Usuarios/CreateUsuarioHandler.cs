using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Usuarios;
public sealed class CreateUsuarioHandler(IUsuarioRepository repo) : IRequestHandler<CreateUsuario, Guid>
{
    public async Task<Guid> Handle(CreateUsuario req, CancellationToken ct)
    {
        var usuario = new Usuario(req.Nombre, req.Correo, req.Rol);

        await repo.AddAsync(usuario, ct);
        return usuario.Id;
    }
}