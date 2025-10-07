using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.Repuestos;
public record RepuestoDto( Guid Id, string Codigo, string Descripcion, int CantidadStock, decimal PrecioUnitario);