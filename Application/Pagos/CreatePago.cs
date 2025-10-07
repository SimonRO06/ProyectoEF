using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using MediatR;

namespace Application.Pagos;
public sealed record CreatePago(decimal Monto, DateTime FechaPago, MetodoPago MetodoPago, Guid FacturaId) : IRequest<Guid>;