using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Marcas;
public sealed record CreateMarca(string Nombre) : IRequest<Guid>;