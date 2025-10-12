using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Cita
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime Fecha { get; private set; }
    public TimeSpan Hora { get; private set; }
    public string Observaciones { get; private set; } = null!;

    public Guid ClienteId { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public Guid VehiculoId { get; set; }
    public virtual Vehiculo? Vehiculo { get; set; }

    private Cita() { }
    public Cita(DateTime fecha, TimeSpan hora, string observaciones, Guid vehiculoId, Guid clienteId)
    { Fecha = fecha; Hora = hora; Observaciones = observaciones; VehiculoId = vehiculoId; ClienteId = clienteId; }

    public void Update(string observaciones)
    { Observaciones = observaciones; }
}