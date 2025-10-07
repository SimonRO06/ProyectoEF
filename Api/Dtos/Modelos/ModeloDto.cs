using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.Modelos;
public record ModeloDto( Guid Id, string Nombre, DateTime FechaIngreso, Guid MarcaId);