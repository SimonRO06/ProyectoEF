using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.DetallesOrdenes;
public record DetalleOrdenDto(Guid Id, int Cantidad, decimal CostoUnitario, Guid OrdenServicioId, Guid RepuestoId);