using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Vehiculos;
public sealed record CreateVehiculo(int Año, string NumeroSerie, int Kilometraje, Guid ClienteId, Guid ModeloId) : IRequest<Guid>;