using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Modelo
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; private set; } = null!;
    public Guid MarcaId { get; set; }
    public virtual Marca? Marca { get; set; }
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new HashSet<Vehiculo>();
    private Modelo() { }
    public Modelo(string nombre, Guid marcaId)
    {
        Nombre = nombre; MarcaId = marcaId;
    }

    public void Update(string nombre)
    {
        Nombre = nombre;
    }
}
