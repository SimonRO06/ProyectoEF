using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Api.Dtos.Pagos;
public record CreatePagoDto( decimal Monto, MetodoPago MetodoPago, Guid FacturaId);