using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entidades;

namespace Domain.Entities;

public class DetalleOrden
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int Cantidad { get; private set; }
    public decimal CostoUnitario { get; private set; }
    public decimal SubTotal { get; private set; }
    public Guid OrdenServicioId { get; set; }
    public virtual OrdenServicio? OrdenServicio { get; set; }
    public Guid RepuestoId { get; set; }
    public virtual Repuesto? Repuesto { get; set; }

    private DetalleOrden() { }
    public DetalleOrden(int cantidad, decimal costounitario, decimal subtotal, Guid ordenServicioId, Guid respuestaId)
    {
        Cantidad = cantidad; CostoUnitario = costounitario; SubTotal = subtotal;
        OrdenServicioId = ordenServicioId;
    }
}
