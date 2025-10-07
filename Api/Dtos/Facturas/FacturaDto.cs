using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.Facturas;
public record FacturaDto(Guid Id, DateTime FechaEmision, decimal Total, Guid OrdenServicioId);