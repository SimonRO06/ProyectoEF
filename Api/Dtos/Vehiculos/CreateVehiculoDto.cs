using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.Vehiculos;
public record CreateVehiculoDto( int Año, string NumeroSerie, int Kilometraje, Guid ClienteId, Guid ModeloId);