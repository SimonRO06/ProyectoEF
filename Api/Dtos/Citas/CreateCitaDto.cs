using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.Citas;
public record CreateCitaDto(DateTime Fecha, TimeSpan Hora, string Observaciones, Guid VehiculoId, Guid ClienteId);