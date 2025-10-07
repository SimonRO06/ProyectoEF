using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Entities;

public class Vehiculo
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int Año { get; private set; }
    public string NumeroSerie { get; private set; } = null!;
    public int Kilometraje { get; private set; }

    public Guid ClienteId { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public Guid ModeloId { get; set; }
    public virtual Modelo? Modelo { get; set; }
    public virtual ICollection<OrdenServicio> OrdenesServicios { get; set; } = new HashSet<OrdenServicio>();
    private Vehiculo() { }
    public Vehiculo(int año, string numeroSerie, int kilometraje, Guid clienteId, Guid modeloId)
    {
        Año = año; NumeroSerie = numeroSerie; Kilometraje = kilometraje;
        ClienteId = clienteId;
        ModeloId = modeloId;
    }

}