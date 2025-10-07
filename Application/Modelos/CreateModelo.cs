using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Modelos;
public sealed record CreateModelo(string Nombre, Guid MarcaId) : IRequest<Guid>;