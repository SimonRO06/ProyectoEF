using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Entities;

public class Vehiculo
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Modelo { get; private set; } = null!;
    public int Año { get; private set; }
    public string NumeroSerie { get; private set; } = null!;
    public int Kilometraje { get; private set; }
    public Guid ClienteId { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public virtual ICollection<OrdenServicio> OrdenesServicios { get; set; } = new HashSet<OrdenServicio>();
    public Guid MarcaId { get; set; }
    public virtual Marca? Marca { get; set; }
    private Vehiculo() { }
    public Vehiculo(string modelo, int año, string numeroSerie, int kilometraje, Guid clienteId, Guid marcaId)
    {
        Modelo = modelo; Año = año; NumeroSerie = numeroSerie; Kilometraje = kilometraje;
        ClienteId = clienteId;
        MarcaId = marcaId;
    }

}