using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.DetallesOrdenes;
public sealed record CreateDetalleOrden(int Cantidad, decimal CostoUnitario, Guid OrdenServicioId, Guid RepuestoId) : IRequest<Guid>;