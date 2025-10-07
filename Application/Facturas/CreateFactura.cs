using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Facturas;
public sealed record CreateFactura(DateTime FechaEmision, decimal Impuestos, decimal Total, Guid OrdenServicioId) : IRequest<Guid>;