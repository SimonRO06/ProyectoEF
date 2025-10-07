using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.DetallesOrdenes;
public record CreateDetalleOrdenDto(int Cantidad, decimal CostoUnitario, Guid OrdenServicioId, Guid RespuestaId);