using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Factura
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime FechaEmision { get; private set; } = DateTime.UtcNow;
    public decimal Impuestos { get; private set; }
    public decimal Total { get; private set; }
    
    public Guid OrdenServicioId { get; set; }
    public virtual OrdenServicio? OrdenServicio { get; set; }
    public virtual ICollection<Pago> Pagos { get; set; } = new HashSet<Pago>();
    private Factura() { }
    public Factura(decimal impuestos, decimal total, Guid ordenServicioId)
    {
        FechaEmision = DateTime.UtcNow; Impuestos = impuestos; Total = total;
        OrdenServicioId = ordenServicioId;
    }

}
