using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entidades;

public class Marca
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new HashSet<Vehiculo>();
}