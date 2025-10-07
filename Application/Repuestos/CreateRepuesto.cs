using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Repuestos;
public sealed record CreateRepuesto(string Codigo, string Descripcion, int CantidadStock, decimal PrecioUnitario) : IRequest<Guid>;