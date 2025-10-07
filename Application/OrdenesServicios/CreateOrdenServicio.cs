using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using MediatR;

namespace Application.OrdenesServicios;
public sealed record CreateOrdenServicio(TipoServicio TipoServicio, DateTime FechaIngreso, DateTime FechaEstimadaEntrega, Estado Estado, Guid UsuarioId, Guid VehiculoId) : IRequest<Guid>;