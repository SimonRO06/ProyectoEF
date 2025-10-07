using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Clientes;
public sealed record CreateCliente(string Nombre, string Telefono, string Correo) : IRequest<Guid>;