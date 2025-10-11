using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Marca
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; private set; } = null!;
    public virtual ICollection<Modelo> Modelos { get; set; } = new HashSet<Modelo>();
    public Marca(string nombre)
    { Nombre = nombre; }

    public void Update(string nombre)
    { Nombre = nombre; }
}