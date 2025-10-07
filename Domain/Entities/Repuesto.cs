using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Entities;

public class Repuesto
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Codigo { get; private set; } = null!;
    public string Descripcion { get; private set; } = null!;
    public int CantidadStock { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public virtual ICollection<DetalleOrden> DetallesOrden { get; set; } = new HashSet<DetalleOrden>();
    private Repuesto() { } // EF
    public Repuesto(string codigo, string descripcion, int cantidadStock, decimal precioUnitario)
    { Codigo = codigo; Descripcion = descripcion; CantidadStock = cantidadStock; PrecioUnitario = precioUnitario;}
}
    
