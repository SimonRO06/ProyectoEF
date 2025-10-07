using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Api.Dtos.Pagos;
public record PagoDto( Guid Id, decimal Monto, DateTime FechaPago, MetodoPago MetodoPago, Guid FacturaId);