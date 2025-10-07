using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.Facturas;

public record UpdateFacturaDto( decimal SubTotal, decimal Impuestos, decimal Total);