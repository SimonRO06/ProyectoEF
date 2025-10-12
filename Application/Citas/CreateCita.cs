using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Citas;
public sealed record CreateCita(DateTime Fecha, TimeSpan Hora, string Observaciones, Guid ClienteId, Guid VehiculoId) : IRequest<Guid>;