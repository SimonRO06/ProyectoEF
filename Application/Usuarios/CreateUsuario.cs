using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using MediatR;

namespace Application.Usuarios;
public sealed record CreateUsuario(string Nombre, string Correo, Rol Rol) : IRequest<Guid>;