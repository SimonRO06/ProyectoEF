using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.Facturas;
public record FacturaDto(Guid Id, DateTime FechaEmision, decimal SubTotal, decimal Impuestos, decimal Total, Guid OrdenServicioId);