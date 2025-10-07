using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Api.Dtos.OrdenesServicios;
public record CreateOrdenServicioDto( TipoServicio TipoServicio, DateTime FechaEstimadaEntrega, Estado Estado, Guid UsuarioId, Guid VehiculoId);