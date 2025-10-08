using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Api.Dtos.OrdenesServicios;
public record OrdenServicioDto( Guid Id, TipoServicio TipoServicio, DateTime FechaIngreso, DateTime FechaEstimadaEntrega, Estado Estado, int UserMemberId, Guid VehiculoId);