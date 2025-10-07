using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities;
public class Pago
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public decimal Monto { get; private set; }
    public DateTime FechaPago { get; private set; } = DateTime.UtcNow;
    public MetodoPago MetodoPago { get; private set; }

    public Guid FacturaId { get; set; }
    public virtual Factura? Factura { get; set; }

    private Pago() { }

    public Pago(decimal monto, MetodoPago metodoPago, Guid facturaId)
    {
        FechaPago = DateTime.UtcNow;
        Monto = monto;
        MetodoPago = metodoPago;
        FacturaId = facturaId;
    }
}
